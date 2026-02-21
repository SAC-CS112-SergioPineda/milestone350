using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CST_350_MilestoneProject.Filters
{
    public class SessionCheckFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                context.Result = new RedirectResult("/User/Login");
            }
        }
    }
}
