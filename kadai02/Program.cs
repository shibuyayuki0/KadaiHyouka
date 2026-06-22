using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using NLog;
using kadai02;

namespace kadai02;

class Program
{
    // 非同期(async Task)でメイン処理を実行可能にします
    static async Task Main(string[] args)
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TASKS;Trusted_Connection=True;";

        logger.Info("タスク統計バッチ処理を開始します。");
        Console.WriteLine("プログラムを開始します。データベースへ接続中...");

        try
        {
            using var connection = new SqlConnection(ConnectionString);
            // データベース接続を確立
            await connection.OpenAsync();

            // Dapperを用いた非同期での全件取得
            var sql = "SELECT * FROM TASKS";
            var taskEnumerable = await connection.QueryAsync<TaskModel>(sql);
            var taskList = new List<TaskModel>(taskEnumerable);

            // 解析クラスのインスタンス生成
            var analyzer = new TaskAnalyzer(logger);

            // 未完了タスクの抽出と一覧表示
            analyzer.DisplayUnfinishedTasks(taskList);

            // 期限切れチェックの実行（Warnログ出力）
            analyzer.CheckOverdueTasks(taskList);

            // 担当者ごとの集計ログ出力（Infoログ出力）
            analyzer.LogTaskCountByAssignee(taskList);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "バッチ処理中に予期せぬエラーが発生しました。");
            Console.WriteLine($"\n【エラー発生】{ex.Message}");
            Console.WriteLine("データベース接続文字列、またはLocalDBが起動しているか確認してください。");
        }

        logger.Info("タスク統計バッチ処理を終了しました。");

        
        Console.WriteLine("\n処理が完了しました。Enterキーを押すと画面を閉じます。");
        Console.ReadLine();
    }
}
