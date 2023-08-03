﻿using SMO;

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Helpers;
using System.Web.Routing;

namespace System.Web.Mvc.Html
{
    public static class MyUtilExtensions
    {
        public const string ATTRIBUTES_CLASS = "class";

        public static string GetAntiForgeryToken(this HtmlHelper helper)
        {
            AntiForgery.GetTokens(null, out string cookieToken, out string formToken);
            return cookieToken + ":" + formToken;
        }

        public static IDictionary<string, object> MergeHtmlAttributes(object htmlAttributes, string strClass)
        {
            IDictionary<string, object> htmlAttr = new Dictionary<string, object>();
            if (htmlAttributes.GetType().Name == "RouteValueDictionary")
            {
                if (htmlAttributes is RouteValueDictionary route)
                {
                    foreach (var item in route)
                    {
                        htmlAttr.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                htmlAttr = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            }

            if (htmlAttr.ContainsKey(ATTRIBUTES_CLASS))
            {
                htmlAttr[ATTRIBUTES_CLASS] = strClass + " " + htmlAttr[ATTRIBUTES_CLASS];
            }
            else
            {
                htmlAttr.Add(ATTRIBUTES_CLASS, strClass);
            }
            return htmlAttr;
        }

        public static string AppendStringValidate(string pStrValue, string pStrClass, string pStrValidate)
        {
            if (string.IsNullOrEmpty(pStrValue))
            {
                return pStrClass;
            }
            string strResult = "validate[";
            strResult = pStrClass + (pStrClass.Equals(strResult) ? string.Format("{0}[{1}]", pStrValidate, pStrValue) : string.Format(",{0}[{1}]", pStrValidate, pStrValue));
            return strResult;
        }
    }

    public static class MyTextBoxExtensions
    {
        /// <summary>
        /// Control TextArea
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString MyTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(new { }, "form-control");
            MvcHtmlString html = htmlHelper.TextAreaFor(expression, htmlAttr);
            return html;
        }

        /// <summary>
        /// Control TextArea có định nghĩa thêm các thuộc tính
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes">Thuộc tính cần thêm. Ví dụ : new {@id = "....", @class = "...."}</param>
        /// <returns></returns>
        public static MvcHtmlString MyTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(htmlAttributes, "form-control");
            MvcHtmlString html = htmlHelper.TextAreaFor(expression, htmlAttr);
            return html;
        }

        public static MvcHtmlString MyTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(new { }, "form-control");
            MvcHtmlString html = htmlHelper.TextBoxFor(expression, htmlAttr);
            return html;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString MyTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(htmlAttributes, "form-control");
            MvcHtmlString html = htmlHelper.TextBoxFor(expression, htmlAttr);
            return html;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static MvcHtmlString MyTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(new { }, "form-control");
            MvcHtmlString html = htmlHelper.TextBoxFor(expression, format, htmlAttr);
            return html;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="format"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString MyTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(htmlAttributes, "form-control");
            MvcHtmlString html = htmlHelper.TextBoxFor(expression, format, htmlAttr);
            return html;
        }

    }

    public static class MyCheckboxExtensions
    {
        public static MvcHtmlString MyCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression, object htmlAttributes)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(htmlAttributes, " form-control ");
            MvcHtmlString html = htmlHelper.CheckBoxFor(expression, htmlAttr);
            return html;
        }
    }

    public static class MySelectExtensions
    {
        public static MvcHtmlString MyDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(new { }, " form-control ");
            MvcHtmlString html = htmlHelper.DropDownListFor(expression, selectList, htmlAttr);
            return html;
        }

        public static MvcHtmlString MyDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(htmlAttributes, " form-control ");
            MvcHtmlString html = htmlHelper.DropDownListFor(expression, selectList, htmlAttr);
            return html;
        }

        public static MvcHtmlString MyDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(new { }, " form-control ");
            MvcHtmlString html = htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttr);
            return html;
        }

        public static MvcHtmlString MyDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            var htmlAttr = MyUtilExtensions.MergeHtmlAttributes(htmlAttributes, " form-control ");
            MvcHtmlString html = htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttr);
            return html;
        }
    }

    public static class MyButtonExtensions
    {
        /// <summary>
        /// Tạo button voi cac icon
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="caption">Caption cua button</param>
        /// <param name="action">Duong link de thuc hien hanh dong nao do</param>
        /// <param name="title">Title text</param>
        /// <param name="classIcon">Kieu icon.</param>
        /// <param name="isShow">Co hien thi hay khong</param>
        /// <param name="typeButton">Kiểu button: default, primary, success, info, warning, danger</param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString MyButton(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, string typeButton = "", bool isShow = true, object htmlAttributes = null)
        {
            var clsButton = "";
            if (string.IsNullOrWhiteSpace(typeButton))
            {
                clsButton = "btn-default";
            }
            else if (typeButton == "border")
            {
                clsButton = "btn-border";
            }
            else if (typeButton == "primary")
            {
                clsButton = "btn-primary";
            }
            else if (typeButton == "success")
            {
                clsButton = "btn-success";
            }
            else if (typeButton == "info")
            {
                clsButton = "btn-info";
            }
            else if (typeButton == "warning")
            {
                clsButton = "btn-warning";
            }
            else if (typeButton == "danger")
            {
                clsButton = "btn-danger";
            }

            TagBuilder tagBuilder = new TagBuilder("span");
            TagBuilder tagBuilder2 = new TagBuilder("i");
            TagBuilder tagBuilder3 = new TagBuilder("span");

            if (!isShow)
            {
                tagBuilder.MergeAttribute("style", "display: none;");
            }
            tagBuilder.MergeAttribute("id", id);
            tagBuilder.MergeAttribute("onclick", action);
            //tagBuilder.MergeAttribute("title", title);
            tagBuilder.MergeAttribute("title", "");
            if (htmlAttributes != null)
            {
                tagBuilder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            }
            tagBuilder.AddCssClass("btn");
            tagBuilder.AddCssClass(clsButton);
            tagBuilder.AddCssClass("waves-effect");
            tagBuilder2.AddCssClass("material-icons");
            tagBuilder2.AddCssClass("col-fecon");
            tagBuilder2.InnerHtml = classIcon;
            tagBuilder3.InnerHtml = caption;
            tagBuilder.InnerHtml = tagBuilder2.ToString() + tagBuilder3.ToString();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(tagBuilder.ToString());
            return MvcHtmlString.Create(stringBuilder.ToString());
        }

        public static MvcHtmlString MyButtonDefault(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, bool isShow = true, object htmlAttributes = null)
        {
            return MyButton(htmlHelper, id, caption, action, title, classIcon, "default", isShow, htmlAttributes);
        }

        public static MvcHtmlString MyButtonPrimary(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, bool isShow = true, object htmlAttributes = null)
        {
            return MyButton(htmlHelper, id, caption, action, title, classIcon, "primary", isShow, htmlAttributes);
        }

        public static MvcHtmlString MyButtonSuccess(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, bool isShow = true, object htmlAttributes = null)
        {
            return MyButton(htmlHelper, id, caption, action, title, classIcon, "success", isShow, htmlAttributes);
        }

        public static MvcHtmlString MyButtonInfo(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, bool isShow = true, object htmlAttributes = null)
        {
            return MyButton(htmlHelper, id, caption, action, title, classIcon, "info", isShow, htmlAttributes);
        }

        public static MvcHtmlString MyButtonWarning(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, bool isShow = true, object htmlAttributes = null)
        {
            return MyButton(htmlHelper, id, caption, action, title, classIcon, "warning", isShow, htmlAttributes);
        }

        public static MvcHtmlString MyButtonDanger(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, bool isShow = true, object htmlAttributes = null)
        {
            return MyButton(htmlHelper, id, caption, action, title, classIcon, "danger", isShow, htmlAttributes);
        }
        /// <summary>
        /// Tạo button voi cac icon, co phan quyen
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="caption">Caption cua button</param>
        /// <param name="action">Duong link de thuc hien hanh dong nao do</param>
        /// <param name="title">Title text</param>
        /// <param name="classIcon">Icon cua button</param>
        /// <param name="formCode">Form code</param>
        /// <param name="isShow">Co hien thi hay khong</param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString MyButton(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, string right, string typeButton = "", bool isShow = true, object htmlAttributes = null)
        {
            var result = MvcHtmlString.Create(string.Empty);
            if (SMOUtilities.CheckRight(right))
            {
                result = MyButton(htmlHelper, id, caption, action, title, classIcon, typeButton, isShow, htmlAttributes);
            }

            //var listRole = new List<T_AD_ROLE_DETAIL_EX>();
            //var isIgnore = ProfileUtilities.User.IS_IGNORE_USER;
            //if (!isIgnore)
            //{
            //    listRole = ProfileUtilities.ListRole as List<T_AD_ROLE_DETAIL_EX>;
            //}

            //if (isIgnore || listRole.Any(x => x.FORM_CODE == formCode & x.CONTROL == id))
            //{
            //    result = MyButton(htmlHelper, id, caption, action, title, classIcon, isShow, htmlAttributes);
            //}

            //result = MyButton(htmlHelper, id, caption, action, title, classIcon, isShow, htmlAttributes);
            return result;
        }

        //public static MvcHtmlString MyButton(this HtmlHelper htmlHelper, string id, string caption, string action, string title, string classIcon, string formCode, string Right, bool isShow = true, object htmlAttributes = null)
        //{
        //    var result = MvcHtmlString.Create(string.Empty);
        //    //if (SMOUtilities.CheckRight(Right, UserRole.USER_RIGHT.ToString()))
        //    //{
        //    //    result = MyButton(htmlHelper, id, caption, action, title, classIcon, isShow, htmlAttributes);
        //    //}
        //    //var listRole = new List<T_AD_ROLE_DETAIL_EX>();
        //    //var isIgnore = ProfileUtilities.User.IS_IGNORE_USER;
        //    //if (!isIgnore)
        //    //{
        //    //    listRole = ProfileUtilities.ListRole as List<T_AD_ROLE_DETAIL_EX>;
        //    //}

        //    //if (isIgnore || listRole.Any(x => x.FORM_CODE == formCode & x.CONTROL == id))
        //    //{
        //    //    result = MyButton(htmlHelper, id, caption, action, title, classIcon, isShow, htmlAttributes);
        //    //}

        //    return result;
        //}
    }
    public static class MyGridExtentions
    {
        public static MvcHtmlString MyGridHeader<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string strclass = "", string style = "", bool isSort = true)
        {
            TagBuilder tagBuilderTh = new TagBuilder("th");
            tagBuilderTh.AddCssClass(strclass);
            tagBuilderTh.MergeAttribute("style", style);
            var strName = string.Empty;
            if (expression.Body is MemberExpression memberExpresstion)
            {
                strName = memberExpresstion.Member.Name;
            }
            TagBuilder tagBuilderLabel = new TagBuilder("text");
            tagBuilderLabel.MergeAttribute("field", strName);
            tagBuilderLabel.MergeAttribute("direction", string.Empty);
            tagBuilderLabel.InnerHtml = DisplayNameExtensions.DisplayNameFor(htmlHelper, expression).ToString();
            TagBuilder tagBuilderSpan = new TagBuilder("span");
            if (isSort)
            {
                tagBuilderTh.MergeAttribute("onclick", "Grids.Sorting(this);");
            }
            tagBuilderTh.InnerHtml = tagBuilderLabel.ToString() + tagBuilderSpan.ToString();
            return MvcHtmlString.Create(tagBuilderTh.ToString());
        }
    }
}