using Dapper;
using kadai01.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient; // SQL Server接続用

namespace kadai01.Pages
{
    public class IndexModel(ITaskDataEditor dataEditor) : PageModel
    {
        // 外部から渡されたオブジェクトを、クラス内で使い回すためのプライベート変数
        private readonly ITaskDataEditor _dataEditor = dataEditor;


        public List<kadai01.Model.Tasks> TaskList { get; set; }

        public List<SelectListItem> FilterList { get; } = [
        new SelectListItem{ Value = ListFilterMode.NotStarted.ToString(), Text = "未着手" },
                new SelectListItem{ Value = ListFilterMode.InProgress.ToString(), Text = "進行中" },
        new SelectListItem{ Value = ListFilterMode.Complete.ToString(), Text = "完了" }
    ];

        // フィルターの選択肢（初期値は未完了）
        [BindProperty(SupportsGet = true)]
        public ListFilterMode SelectedFilter { get; set; } = ListFilterMode.NotStarted;


        public IActionResult OnGet()
        {
            TaskList = _dataEditor.GetTaskList(SelectedFilter);
            return Page();
        }

    }
}
