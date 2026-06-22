using Dapper;
using kadai01.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient; // SQL Server接続用
using System.ComponentModel.DataAnnotations;


namespace kadai01.Pages;

public class EditModel(ITaskDataEditor dataEditor) : PageModel
{
    private readonly ITaskDataEditor _dataEditor = dataEditor;

    public List<SelectListItem>? Options { get; private set; }

    // ===== 入力欄 =====

    // タスク名
    [BindProperty]
    [Display(Name = "タスク名")]
    [Required(ErrorMessage = "タスク名は必須項目です。")]
    [StringLength(100, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    public string? TASK_NAME { get; set; }

    [BindProperty]
    [Display(Name = "担当者名")]
    [Required(ErrorMessage = "担当者名は必須項目です。")]
    [StringLength(50, ErrorMessage = "{0}は{1}文字以内で入力してください。")]
    public string? ASSIGNEE { get; set; }

    [BindProperty]
    [Display(Name = "期限日")]
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "期限は必須項目です。")]
    public DateTime? DUE_DATE { get; set; }

    [BindProperty]
    [Display(Name = "ステータス")]
    [Required(ErrorMessage = "ステータスは必須入力です。")]
    public string? STATUS { get; set; }

    // ===== 外観 =====

    // 登録画面の説明
    [BindProperty]
    public string FormTitle { get; set; } = "タスクの編集";

    // 登録ボタンの名称
    [BindProperty]
    public string PostButtonName { get; set; } = "編集";


    public IActionResult OnGet(string? id)
    {
        // --- DB接続 ---
        //try
        //{
        //    // 優先度リストをプルダウンメニューとして組立
        //    SetSelectList();
        //}
        //catch (SqlException)
        //{
        //    return StatusCode(500, "データベース接続に失敗しました。管理者に問い合わせてください。");
        //}

        if (!string.IsNullOrEmpty(id))
        {
            // IDが不正（文字列や0以下）なら400エラー
            if (!int.TryParse(id, out var parsedId) || parsedId <= 0)
            {
                return BadRequest("無効なIDです。");
            }

            Tasks? targetTask;
            // --- DB接続 ---
            try
            {
                // タスクを取得
                targetTask = _dataEditor.GetTasks(parsedId);
            }
            catch (SqlException)
            {
                return StatusCode(500, "1データベース接続に失敗しました。管理者に問い合わせてください。");
            }

            // IDが存在しなければ404エラー
            if (targetTask is null)
            {
                return NotFound("タスクは存在しません。");
            }

            // 各入力欄に初期値を入れる
            TASK_NAME = targetTask.TASK_NAME;
            ASSIGNEE = targetTask.ASSIGNEE;
            STATUS = targetTask.STATUS;
            DUE_DATE = targetTask.DUE_DATE;

        }

        // ページを表示
        return Page();
    }

    public IActionResult OnPost(int? id)
    {
        // 必須入力欄がないとき
        if (!ModelState.IsValid)
        {
            // --- DB接続 ---
            try
            {

            }
            catch (SqlException)
            {
                return StatusCode(500, "2データベース接続に失敗しました。管理者に問い合わせてください。");
            }

            // 再表示　→入力エラーが表示される
            return Page();
        }

        Tasks? targetTask;
        try
        {
            // 対象タスク状態を取得する
            targetTask = _dataEditor.GetTasks(id.Value);
        }
        catch (SqlException)
        {
            return StatusCode(500, "3データベース接続に失敗しました。管理者に問い合わせてください。");
        }

        // タスクが削除されていたら
        if (targetTask is null)
        {
            return BadRequest("指定されたタスクは、他のユーザーによって削除されました。");
        }

        // 更新用データを作成する
        // targetTask!：idの存在はページ表示時点で保証されているので、nullチェックをしない
        var updateTask = targetTask with
        {
            TASK_NAME = this.TASK_NAME!,
            ASSIGNEE = this.ASSIGNEE!,
            STATUS = this.STATUS!,
            DUE_DATE = DUE_DATE!,
        };

        // --- DB接続 ---
        try
        {
            // タスクを更新する
            _dataEditor.Update(updateTask);
        }
        catch (SqlException ex)
        {
            return StatusCode(500, $"4のエラー内容: {ex.Message}");

        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest($"{ex.Message}"); // タスクIDxxは存在しない、または既に削除されています。
        }


        // リダイレクト：Indexに戻る
        return RedirectToPage("/Index");
    }
}

