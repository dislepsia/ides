using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace cnrl.Helpers
{
    public static class MvcExtensions
    {

        public static List<SelectListItem> ToSelectList<T>(
            this IEnumerable<T> enumerable,
            Func<T, string> text,
            Func<T, string> value,
            string defaultOption = null,
            string defaultValue = null,
            string selectedValue = null
            )
            {
            var items = enumerable.Select(f => new SelectListItem
            {
                Text = text(f),
                Value = value(f),
                Selected = selectedValue != null && value(f) == selectedValue
            }).ToList();
            if (!string.IsNullOrEmpty(defaultOption))
            {
                items.Insert(0, new SelectListItem
                {
                    Text = defaultOption,
                    Value = defaultValue,
                    Selected = selectedValue != null && defaultValue == selectedValue
                });                
            }
            return items;
        }

        public static SelectList ToSelectList2<T>(
            this IEnumerable<T> enumerable,
            Func<T, string> text,
            Func<T, string> value,
            string defaultOption = null,
            string defaultValue = null,
            string selectedValue = null
        )
        {
            var items = enumerable.Select(f => new SelectListItem
            {
                Text = text(f),
                Value = value(f),
                Selected = selectedValue != null && value(f) == selectedValue
            }).ToList();
            if (!string.IsNullOrEmpty(defaultOption))
            {
                items.Insert(0, new SelectListItem
                {
                    Text = defaultOption,
                    Value = defaultValue,
                    Selected = selectedValue != null && defaultValue == selectedValue
                });
            }
            return new SelectList(items, "Value", "Text");
        }
    }
}