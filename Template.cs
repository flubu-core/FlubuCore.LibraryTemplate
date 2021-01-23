using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Templating;
using FlubuCore.Templating.Models;

public class Template : IFlubuTemplate
{
    public void ConfigureTemplate(IFlubuTemplateBuilder templateBuilder)
    {
        templateBuilder.AddReplacementToken(new TemplateReplacementToken()
        {
            Token = "{{SolutionFileName}}",
            Description = "Enter relative path to solution filename:",
            InputType = InputType.Files,
            Files = new FilesInputType()
            {
                AllowedFileExtension = "sln"
            }
        });
    }
}
