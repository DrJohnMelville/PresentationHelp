using Melville.Lists;
using PresentationHelp.ScreenInterface;
using PresentationHelp.WpfViewParts;
using System.Buffers.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PresentationHelp.DocumentScanner;

public class SingleScannedDocument {
    public BitmapImage Image { get; }
    private byte[] imageData;
    private const int dataUrlHeaderLength = 22;
    public SingleScannedDocument(string source)
    {
        int length = Base64.GetMaxDecodedFromUtf8Length(source.Length - dataUrlHeaderLength);
        Convert.TryFromBase64Chars(source.AsSpan(dataUrlHeaderLength), 
            this.imageData = new byte[length], out int bytesWritten);

        Image = new BitmapImage();
        Image.BeginInit();
        Image.StreamSource = new MemoryStream(this.imageData);
        Image.EndInit();
        Image.Freeze();
    }
}
public class ScanDocumentViewModel : IScreenDefinition
{
    public ThreadSafeBindableCollection<SingleScannedDocument> ScannedDocuments { get; } = new();
    public object PublicViewModel => this;

    public object CommandViewModel => this;

    public string CommandGroupTitle => "Scan Document";

    public IEnumerable<ICommandInfo> Commands => [];

    public ValueTask<CommandResult> TryParseCommandAsync(string command, IScreenHolder holder)
    {
        return ValueTask.FromResult(new CommandResult
        {
            NewScreen = holder.Screen,
            Result = CommandResultKind.NotRecognized
        });
    }

    public ValueTask AcceptDatum(string user, string datum)
    {
        //data:image/png;base64,iVBORw0KGg -- skip 22 characters
        ScannedDocuments.Add(new SingleScannedDocument(datum));
        return ValueTask.CompletedTask;
    }

    public string HtmlForUser(IHtmlBuilder builder) =>
        builder.CommonClientPage(
    """
    <style>
        body {
            font-family: sans-serif;
            padding: 20px;
            max-width: 900px;
            margin: auto;
        }

        canvas,
        video {
            display: block;
            margin-top: 20px;
            border: 1px solid #ccc;
            max-width: 100%;
            height: auto;
        }

        pre {
            background: #f0f0f0;
            padding: 10px;
            margin-top: 10px;
            white-space: pre-wrap;
        }

        button.save-btn,
        button#startCameraBtn,
        button#captureBtn {
            margin-top: 10px;
            padding: 8px 16px;
            cursor: pointer;
            background-color: #007bff;
            color: white;
            border: none;
            border-radius: 4px;
            font-size: 14px;
        }

        #cameraContainer {
            margin-top: 40px;
        }
    </style>
    """, """
    <div>
        <div id="cameraContainer" style="display:none;">
        <video id="video" autoplay playsinline style="display:none"></video>
        <canvas id="highlightCanvas"></canvas>
        <button id="captureBtn">📸 Capture Scanned Document</button>
        <div id="cameraResult"></div>
    </div>

    <script async src="https://docs.opencv.org/4.x/opencv.js"></script>
    <script>
    (function (global, factory) {
        typeof exports === "object" && typeof module !== "undefined"
            ? (module.exports = factory())
            : typeof define === "function" && define.amd
                ? define(factory)
                : (global.jscanify = factory());
    })(this, function () {
        "use strict";

        /**
         * Calculates distance between two points. Each point must have `x` and `y` property
         * @param {*} p1 point 1
         * @param {*} p2 point 2
         * @returns distance between two points
         */
        function distance(p1, p2) {
            return Math.hypot(p1.x - p2.x, p1.y - p2.y);
        }

        class jscanify {
            constructor() { }

            /**
             * Finds the contour of the paper within the image
             * @param {*} img image to process (cv.Mat)
             * @returns the biggest contour inside the image
             */
            findPaperContour(img) {
                const imgGray = new cv.Mat();
                cv.Canny(img, imgGray, 50, 200);

                const imgBlur = new cv.Mat();
                cv.GaussianBlur(
                    imgGray,
                    imgBlur,
                    new cv.Size(3, 3),
                    0,
                    0,
                    cv.BORDER_DEFAULT
                );

                const imgThresh = new cv.Mat();
                cv.threshold(
                    imgBlur,
                    imgThresh,
                    0,
                    255,
                    cv.THRESH_OTSU
                );

                let contours = new cv.MatVector();
                let hierarchy = new cv.Mat();

                cv.findContours(
                    imgThresh,
                    contours,
                    hierarchy,
                    cv.RETR_CCOMP,
                    cv.CHAIN_APPROX_SIMPLE
                );

                let maxArea = 0;
                let maxContourIndex = -1;
                for (let i = 0; i < contours.size(); ++i) {
                    let contourArea = cv.contourArea(contours.get(i));
                    if (contourArea > maxArea) {
                        maxArea = contourArea;
                        maxContourIndex = i;
                    }
                }

                const maxContour =
                    maxContourIndex >= 0 ?
                        contours.get(maxContourIndex) :
                        null;

                imgGray.delete();
                imgBlur.delete();
                imgThresh.delete();
                contours.delete();
                hierarchy.delete();
                return maxContour;
            }

            /**
             * Highlights the paper detected inside the image.
             * @param {*} image image to process
             * @param {*} options options for highlighting. Accepts `color` and `thickness` parameter
             * @returns `HTMLCanvasElement` with original image and paper highlighted
             */

            priorCorners = [0, 0, 0, 0, 0, 0, 0, 0];

            cornersStable(newCorners) {
                const oldCorners = this.priorCorners;
                this.priorCorners = newCorners;
                for (let i = 0; i < newCorners.length; i++) {
                    if (Math.abs(newCorners[i] - oldCorners[i]) > 5) {
                        return false;
                    }
                }
                return true;
            }

            highlightPaper(image, capturePhoto, shouldCapture) {

                const canvas = document.createElement("canvas");
                const ctx = canvas.getContext("2d");
                const img = cv.imread(image);

                const maxContour = this.findPaperContour(img);
                cv.imshow(canvas, img);
                if (maxContour) {
                    const {
                        topLeftCorner,
                        topRightCorner,
                        bottomLeftCorner,
                        bottomRightCorner,
                    } = this.getCornerPoints(maxContour, img);

                    if (
                        topLeftCorner &&
                        topRightCorner &&
                        bottomLeftCorner &&
                        bottomRightCorner
                    ) {
                        if (this.cornersStable(
                            [
                                topLeftCorner.x, topLeftCorner.y,
                                topRightCorner.x, topRightCorner.y,
                                bottomLeftCorner.x, bottomLeftCorner.y,
                                bottomRightCorner.x, bottomRightCorner.y,
                            ])) {
                            capturePhoto()
                        }
                        ctx.strokeStyle = shouldCapture? "red": "blue";
                        ctx.lineWidth = 1;
                        ctx.beginPath();
                        ctx.moveTo(...Object.values(topLeftCorner));
                        ctx.lineTo(...Object.values(topRightCorner));
                        ctx.lineTo(...Object.values(bottomRightCorner));
                        ctx.lineTo(...Object.values(bottomLeftCorner));
                        ctx.lineTo(...Object.values(topLeftCorner));
                        ctx.stroke();
                    }
                }

                img.delete();
                return canvas;
            }

            /**
             * Extracts and undistorts the image detected within the frame.
             * 
             * Returns `null` if no paper is detected.
             *  
            * @param {*} image image to process
             * @param {*} resultWidth desired result paper width
             * @param {*} resultHeight desired result paper height
             * @param {*} cornerPoints optional custom corner points, in case automatic corner points are incorrect
             * @returns `HTMLCanvasElement` containing undistorted image
             */
            extractPaper(image, cornerPoints) {
                const canvas = document.createElement("canvas");
                const img = cv.imread(image);
                const maxContour = cornerPoints ? null : this.findPaperContour(img);

                if (maxContour == null && cornerPoints === undefined) {
                    return null;
                }

                const {
                    topLeftCorner,
                    topRightCorner,
                    bottomLeftCorner,
                    bottomRightCorner,
                } = cornerPoints || this.getCornerPoints(maxContour, img);
                let warpedDst = new cv.Mat();

                let resultWidth = 2 * Math.max(topRightCorner.x - topLeftCorner.x, bottomRightCorner.x - bottomLeftCorner.x);
                let resultHeight = 2 * Math.max(bottomLeftCorner.y - topLeftCorner.y, bottomRightCorner.y - topRightCorner.y);

                let dsize = new cv.Size(resultWidth, resultHeight);
                let srcTri = cv.matFromArray(4, 1, cv.CV_32FC2, [
                    topLeftCorner.x,
                    topLeftCorner.y,
                    topRightCorner.x,
                    topRightCorner.y,
                    bottomLeftCorner.x,
                    bottomLeftCorner.y,
                    bottomRightCorner.x,
                    bottomRightCorner.y,
                ]);

                let dstTri = cv.matFromArray(4, 1, cv.CV_32FC2, [
                    0,
                    0,
                    resultWidth,
                    0,
                    0,
                    resultHeight,
                    resultWidth,
                    resultHeight,
                ]);

                let M = cv.getPerspectiveTransform(srcTri, dstTri);
                cv.warpPerspective(
                    img,
                    warpedDst,
                    M,
                    dsize,
                    cv.INTER_LINEAR,
                    cv.BORDER_CONSTANT,
                    new cv.Scalar()
                );

                cv.imshow(canvas, warpedDst);

                img.delete()
                warpedDst.delete()
                return canvas;
            }

            /**
             * Calculates the corner points of a contour.
             * @param {*} contour contour from {@link findPaperContour}
             * @returns object with properties `topLeftCorner`, `topRightCorner`, `bottomLeftCorner`, `bottomRightCorner`, each with `x` and `y` property
             */
            getCornerPoints(contour) {
                let rect = cv.minAreaRect(contour);
                const center = rect.center;

                let topLeftCorner;
                let topLeftCornerDist = 0;

                let topRightCorner;
                let topRightCornerDist = 0;

                let bottomLeftCorner;
                let bottomLeftCornerDist = 0;

                let bottomRightCorner;
                let bottomRightCornerDist = 0;

                for (let i = 0; i < contour.data32S.length; i += 2) {
                    const point = { x: contour.data32S[i], y: contour.data32S[i + 1] };
                    const dist = distance(point, center);
                    if (point.x < center.x && point.y < center.y) {
                        // top left
                        if (dist > topLeftCornerDist) {
                            topLeftCorner = point;
                            topLeftCornerDist = dist;
                        }
                    } else if (point.x > center.x && point.y < center.y) {
                        // top right
                        if (dist > topRightCornerDist) {
                            topRightCorner = point;
                            topRightCornerDist = dist;
                        }
                    } else if (point.x < center.x && point.y > center.y) {
                        // bottom left
                        if (dist > bottomLeftCornerDist) {
                            bottomLeftCorner = point;
                            bottomLeftCornerDist = dist;
                        }
                    } else if (point.x > center.x && point.y > center.y) {
                        // bottom right
                        if (dist > bottomRightCornerDist) {
                            bottomRightCorner = point;
                            bottomRightCornerDist = dist;
                        }
                    }
                }

                return {
                    topLeftCorner,
                    topRightCorner,
                    bottomLeftCorner,
                    bottomRightCorner,
                };
            }
        }

        return jscanify;
    }); window.addEventListener("load", () => {
        const scanner = new jscanify();

        // --- LIVE DETECTION ---
        const cameraContainer = document.getElementById("cameraContainer");
        const video = document.getElementById("video");
        const highlightCanvas = document.getElementById("highlightCanvas");
        const captureBtn = document.getElementById("captureBtn");
        const cameraResult = document.getElementById("cameraResult");
        let stream = null;
        let highlightInterval = null;
        let shouldCapture = false;

        async function openVideo() {
            if (stream) {
                // Stop camera if running
                stream.getTracks().forEach(track => track.stop());
                stream = null;
                cameraContainer.style.display = "none";
                clearInterval(highlightInterval);
                return;
            }

            try {
                stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } });
                video.srcObject = stream;
                await video.play();
                cameraContainer.style.display = "block";

                const ctx = highlightCanvas.getContext("2d");

                // Resize canvas to video size
                highlightCanvas.width = video.videoWidth;
                highlightCanvas.height = video.videoHeight;

                // Continuous highlight loop
                highlightInterval = setInterval(() => {
                    ctx.drawImage(video, 0, 0, highlightCanvas.width, highlightCanvas.height);
                    const hlCanvas = scanner.highlightPaper(highlightCanvas, captureImage, shouldCapture);
                    // Clear and draw highlight result on highlightCanvas
                    ctx.clearRect(0, 0, highlightCanvas.width, highlightCanvas.height);
                    ctx.drawImage(hlCanvas, 0, 0);
                }, 100); // 10 fps approx

            } catch (err) {
                alert("Error accessing camera: " + err.message);
            }

        }

        function captureImage() {
            if (!stream) return;
            if (!shouldCapture) return;
            shouldCapture = false;

            // Extract paper from highlightCanvas (which has current video frame)
            const scan = scanner.extractPaper(highlightCanvas);
            sendDatum(scan.toDataURL("image/png"));
        }

        captureBtn.addEventListener("click", () => shouldCapture = true);
        openVideo();
    });
    </script>
    """);
}