using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace SportEvents.Web.TagHelpers
{
    [HtmlTargetElement("input-phone")]
    public class InputPhoneTagHelper : TagHelper
    {
        private readonly IHtmlGenerator generator;

        public InputPhoneTagHelper(IHtmlGenerator generator)
        {
            this.generator = generator;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression? AspFor { get; set; } = default!;


        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public string? Label { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "form-floating mb-2 phone-input-wrapper");

            string labelText;
            string name;
            ModelExplorer? explorer = null;
            object? value = null;

            if (AspFor != null)
            {
                labelText = Label ?? AspFor.Metadata.DisplayName ?? AspFor.Name;
                name = AspFor.Name;
                explorer = AspFor.ModelExplorer;
                value = AspFor.Model;
            }
            else
            {
                labelText = Label ?? "Телефон";
                name = "phone";
            }

            var input = generator.GenerateTextBox(
                ViewContext,
                explorer,
                name,
                value,
                null,
                new
                {
                    @class = "form-control phone-input",
                    type = "tel",
                    placeholder = labelText
                });

            output.Content.AppendHtml(input);

            output.Content.AppendHtml(
                $"<label class=\"form-label\">{labelText}</label>"
            );

            if (AspFor != null)
            {
                var validation = generator.GenerateValidationMessage(
                    ViewContext,
                    AspFor.ModelExplorer,
                    AspFor.Name,
                    null,
                    null,
                    new { @class = "text-danger d-block mt-1" });

                output.Content.AppendHtml(validation);
            }
        }

    }
}
