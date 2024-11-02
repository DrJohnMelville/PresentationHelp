namespace PresentationHelp.Website.Models.Entities;

public static class CommonCssDefinition {
    public static readonly byte[] Css = """
        html, body {
            height : 99%;
            margin : 0;
            background: rgb(9,4,255);
            background: linear-gradient(317deg, rgba(9,4,255,1) 0%, rgba(163,170,246,1) 100%);
            overflow:hidden;
        }
        
        .smallMargin {
            margin: 15px;
        }
        """u8.ToArray();

}