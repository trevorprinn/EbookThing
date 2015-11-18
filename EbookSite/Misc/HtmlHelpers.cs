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
  
        public static MvcHtmlString ComboBox(this HtmlHelper html, string name, SelectList items, string selectedValue) {
            StringBuilder sb = new StringBuilder();
            sb.Append(html.DropDownList(name + "_hidden", items, new { @style = "width: 200px;", @onchange = "$('input#" + name + "').val($(this).children(':selected').text());" }));
            sb.Append(html.TextBox(name, selectedValue, new { @style = "margin-left: -199px; width: 179px; height: 1.2em; border: 0;" }));
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString ComboBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, SelectList items) {
            MemberExpression me = (MemberExpression)expression.Body;
            string name = me.Member.Name;

            StringBuilder sb = new StringBuilder();
            sb.Append(html.DropDownList(name + "_hidden", items, new { @style = "width: 200px;", @onchange = "$('input#" + name + "').val($(this).children(':selected').text());" }));
            sb.Append(html.TextBoxFor(expression, new { @style = "margin-left: -199px; width: 179px; height: 1.2em; border: 0;" }));
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}