using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace EbookSite {
    // Originally from http://veadas.net/article/aspnet-mvc-combo-box
    public static class ComboBoxHelpers {
  
        public static MvcHtmlString ComboBox(this HtmlHelper html, string name, SelectList items, object htmlAttributes = null, bool allowAny = true, string prependNewValue = "", string appendNewValue = "") {
            StringBuilder sb = new StringBuilder();
            sb.Append(html.DropDownList(name, items, htmlAttributes));
            sb.Append($"<script>$('#{name}').combobox({{allowAny:{allowAny.ToString().ToLower()}, prependNewValue:'{prependNewValue}', appendNewValue:'{appendNewValue}'}});</script>");
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString ComboBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, SelectList items, object htmlAttributes = null, bool allowAny = true, string prependNewValue = "", string appendNewValue = "") {
            MemberExpression me = (MemberExpression)expression.Body;
            string name = me.Member.Name;

            StringBuilder sb = new StringBuilder();
            sb.Append(html.DropDownList(name, items, htmlAttributes));
            sb.Append($"<script>$('#{name}').combobox({{allowAny:{allowAny.ToString().ToLower()}, prependNewValue:'{prependNewValue}', appendNewValue:'{appendNewValue}'}});</script>");
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}