using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace cnrl.Helpers
{
    public static class HtmlExtensions

    {
        public static MvcHtmlString EnableLocalizedClientValidation(this HtmlHelper html)
        {
            // Habilitamos la validación en cliente
            html.EnableClientValidation();

            // Obtenemos la información de la cultura actual
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //var cultureInfo = Thread.CurrentThread.CurrentCulture;

            var cultureInfo = new CultureInfo("en");
            //string name = cultureInfo.Name;
            string name = "en-US";
            string numberFormat = jss.Serialize(cultureInfo.NumberFormat);
            string dateFormat = jss.Serialize(cultureInfo.DateTimeFormat);

            // Generamos el código
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type=\"text/javascript\">");
            sb.AppendLine("var ci = {");
            sb.AppendLine("    \"name\": \"" + name + "\",");
            sb.AppendLine("    \"numberFormat\": " + numberFormat + ", ");
            sb.AppendLine("    \"dateTimeFormat\": " + dateFormat);
            sb.AppendLine("};");
            sb.AppendLine("Sys.CultureInfo.CurrentCulture = Sys.CultureInfo._parse(ci);");
            sb.AppendLine("</script>");
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString LabelForNormalizada<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)

        {

            ModelMetadata meta = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var htmlFullName = ExpressionHelper.GetExpressionText(expression);

            var labelText = meta.DisplayName ?? meta.PropertyName;

            if (String.IsNullOrEmpty(labelText))

            {

                return MvcHtmlString.Empty;

            }

            var tag = new TagBuilder("label");

            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFullName));

            if (htmlAttributes != null)

            {

                tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            }

            Regex r = new Regex(
                @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
              );
            tag.SetInnerText(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(r.Replace(labelText, " ")));

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));

        }

        public static MvcHtmlString LabelForNormalizada<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes)
        {

            ModelMetadata meta = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var htmlFullName = ExpressionHelper.GetExpressionText(expression);

            // var labelText = meta.DisplayName ?? meta.PropertyName;

            if (String.IsNullOrEmpty(labelText))

            {

                return MvcHtmlString.Empty;

            }

            var tag = new TagBuilder("label");

            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFullName));

            if (htmlAttributes != null)

            {

                tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            }

            Regex r = new Regex(
                @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])"
              );
            tag.SetInnerText(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(r.Replace(labelText, " ")));

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));

        }

    }
}