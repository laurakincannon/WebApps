#pragma checksum "T:\public_html\ASP\WDLMassage - Copy\Views\Home\StaffInfo.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "e40aebccf7c66ccff8983cfca3087b8d87994629"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_StaffInfo), @"mvc.1.0.view", @"/Views/Home/StaffInfo.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "T:\public_html\ASP\WDLMassage - Copy\Views\_ViewImports.cshtml"
using WDLMassage;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "T:\public_html\ASP\WDLMassage - Copy\Views\_ViewImports.cshtml"
using WDLMassage.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"e40aebccf7c66ccff8983cfca3087b8d87994629", @"/Views/Home/StaffInfo.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d2746657be31d578f9ea4e926568be5978ba8fa3", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_StaffInfo : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<WDLMassage.Models.User>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "T:\public_html\ASP\WDLMassage - Copy\Views\Home\StaffInfo.cshtml"
  
    ViewData["Title"] = "StaffInfo";

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<h1>Weston Distance Learning Massage Staff</h1>

<table class=""table table-bordered table-striped"">
    <thead>
        <tr>
            <th>
                Name
            </th>
            <th>
                Email Address
            </th>
            <th>
                Phone Number
            </th>
            <th>
                Status
            </th>
        </tr>
    </thead>
    <tbody>
");
#nullable restore
#line 27 "T:\public_html\ASP\WDLMassage - Copy\Views\Home\StaffInfo.cshtml"
         foreach (var item in Model)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 31 "T:\public_html\ASP\WDLMassage - Copy\Views\Home\StaffInfo.cshtml"
               Write(Html.DisplayFor(modelItem => item.NameFull));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 34 "T:\public_html\ASP\WDLMassage - Copy\Views\Home\StaffInfo.cshtml"
               Write(Html.DisplayFor(modelItem => item.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 37 "T:\public_html\ASP\WDLMassage - Copy\Views\Home\StaffInfo.cshtml"
               Write(Html.DisplayFor(modelItem => item.FormattedPhone));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 40 "T:\public_html\ASP\WDLMassage - Copy\Views\Home\StaffInfo.cshtml"
               Write(Html.DisplayFor(modelItem => item.ActiveStatus));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 43 "T:\public_html\ASP\WDLMassage - Copy\Views\Home\StaffInfo.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<WDLMassage.Models.User>> Html { get; private set; }
    }
}
#pragma warning restore 1591