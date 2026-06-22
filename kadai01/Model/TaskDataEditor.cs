using Dapper;
using Microsoft.Data.SqlClient;

namespace kadai01.Model;

public class TaskDataEditor : ITaskDataEditor
{
    // アプリケーション構成プロパティの参照
    private readonly IConfiguration _config;

    // DB接続文字列
    private readonly string? _connectionString;

    public TaskDataEditor(IConfiguration config)
    {
        // DBの接続設定
        _config = config;
        _connectionString = _config.GetConnectionString("DefaultConnection");
    }

    public List<Tasks> GetTaskList(ListFilterMode mode)
    {
        // タスクを全行取得する
        var sql = @"
            SELECT
                TASK_ID AS [TASK_ID],
                TASK_NAME AS [TASK_NAME],
                ASSIGNEE AS [ASSIGNEE],
                DUE_DATE AS [DUE_DATE],
                STATUS AS [STATUS],
                CREATE_DATETIME AS [CREATE_DATETIME],
                UPDATE_DATETIME AS [UPDATE_DATETIME]
            FROM TASKS;";

        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // List型に変換して返す
        var taskList = connection.Query<Tasks>(sql).ToList();

        var filteredList = taskList.Where(l => l.STATUS == mode.ToString()).ToList();
        return [.. taskList];
        // ①スプレッド演算子(..)で、taskListの戻り値をバラバラにする
        // ②コレクション式（[]）で、List型として再構成される
        // ⇒ 今回は戻り値の型がList型なので、[]はList型を返す（型推論）
    }

    public Tasks? GetTasks(int taskId)
    {
        // 1個のタスクを取得するsql文
        var sql = @"
            SELECT
                TASK_ID AS [TASK_ID],
                TASK_NAME AS [TASK_NAME],
                ASSIGNEE AS [ASSIGNEE],
                DUE_DATE AS [DUE_DATE],
                STATUS AS [STATUS],
                CREATE_DATETIME AS [CREATE_DATETIME],
                UPDATE_DATETIME AS [UPDATE_DATETIME]
            FROM TASKS
            WHERE TASK_ID = @TASK_ID;
        ";

        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // ヒットしたタスクを返す、ヒットしないときはNULLを返す
        var task = connection.QueryFirstOrDefault<Tasks>(sql, new { TASK_ID = taskId });
        return task;
    }


    public void Update(Tasks targetTask)
    {
        // タスクIDを取得する
        int taskId = targetTask.TASK_ID;
        
        // タスクを更新するsql文
        var sql = @"
            UPDATE TASKS
            SET
                TASK_NAME = @TASK_NAME,
                ASSIGNEE = @ASSIGNEE,
                STATUS = @STATUS,
                DUE_DATE = @DUE_DATE
                WHERE TASK_ID = @TASK_ID;
        ";
        Console.WriteLine("1");
        // === ここからDB接続 ===
        using var connection = new SqlConnection(_connectionString);

        // sqlを実行し、実際に影響を受けた件数を取得する
        var affectedTasks = connection.Execute(
            sql,
            new
            {
                targetTask.TASK_NAME,
                targetTask.ASSIGNEE,
                targetTask.STATUS,
                targetTask.DUE_DATE,
                TASK_ID = taskId
            });
        Console.WriteLine("2");
        // 件数が0のとき（影響を受けたタスクがない）は、taskIDがおかしいと表示
        if (affectedTasks == 0)
        {
            throw new KeyNotFoundException($"ID：{taskId}というタスクは存在しない、または既に削除されています");
        }
    }
}
