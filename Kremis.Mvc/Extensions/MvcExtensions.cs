using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Collections.Generic;
using Kremis.Domain.Entities;
using Kremis.Areas.Admin.Controllers;
using Kremis.Areas.Operations.Controllers;

namespace Kremis.Mvc.Extensions
{
    public static class MvcExtensions
    {
        public static string ActiveClass(this IHtmlHelper htmlHelper, string areas = null, string controllers = null, string actions = null,
            string cssClass = "active")
        {
            var currentArea = htmlHelper?.ViewContext.RouteData.Values["area"] as string;
            var currentController = htmlHelper?.ViewContext.RouteData.Values["controller"] as string;
            var currentAction = htmlHelper?.ViewContext.RouteData.Values["action"] as string;

            var acceptedAreas = (areas ?? currentArea ?? "").Split(',');
            var acceptedControllers = (controllers ?? currentController ?? "").Split(',');
            var acceptedActions = (actions ?? currentAction ?? "").Split(',');

            return (acceptedAreas.Contains(currentArea)
                && acceptedControllers.Contains(currentController)
                && acceptedActions.Contains(currentAction))
                ? cssClass
                : "";
        }

        public static string CollapseShow(this IHtmlHelper htmlHelper, string areas = null, string controllers = null,
            string cssClass = "show")
        {
            var currentArea = htmlHelper?.ViewContext.RouteData.Values["area"] as string;
            var currentController = htmlHelper?.ViewContext.RouteData.Values["controller"] as string;

            var acceptedAreas = (areas ?? currentArea ?? "").Split(',');
            var acceptedControllers = (controllers ?? currentController ?? "").Split(',');

            return (acceptedAreas.Contains(currentArea)
                && acceptedControllers.Contains(currentController))
                ? cssClass
                : "";
        }
    }
}
