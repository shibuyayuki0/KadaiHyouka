using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace kadai02
{
    public class TaskAnalyzer(NLog.Logger logger)
    {
        private readonly NLog.Logger _logger = logger;

        // 【仕様1】未完了タスクの抽出・一覧表示
        public void DisplayUnfinishedTasks(List<TaskModel> tasks)
        {
            // ステータスが「完了」以外のタスクを抽出
            var unfinishedTasks = tasks.Where(t => t.STATUS != "完了").ToList();

            Console.WriteLine($"【未完了タスク一覧】（件数: {unfinishedTasks.Count}件）");

            foreach (var task in unfinishedTasks)
            {
                Console.WriteLine($"ID: {task.TASK_ID} | タスク名: {task.TASK_NAME} | 担当: {task.ASSIGNEE} | 期限: {task.DUE_DATE:yyyy/MM/dd} | ステータス: {task.STATUS}");
            }
            Console.WriteLine("\n");
        }

        // 仕様2期限切れの警告通知

        public void CheckOverdueTasks(List<TaskModel> tasks)
        {
            _logger.Info("期限切れタスクのチェックを開始します。");

            foreach (var task in tasks)
            {
                // 未完了タスクを対象とする
                if (task.STATUS != "完了")
                {
                    // 期限日（DUE_DATE）が本日（DateTime.Today）を過ぎている（過去の日付）
                    if (task.DUE_DATE < DateTime.Today)
                    {
                        // 適切なログレベル（Warn）で警告ログを出力
                        _logger.Warn($"【警告】期限切れタスクを発見: {task.TASK_NAME} (担当: {task.ASSIGNEE}, 期限: {task.DUE_DATE:yyyy/MM/dd})");

                        // コンソールにもリアルタイムで表示
                        Console.WriteLine($"[WARN] 【警告】期限切れ: {task.TASK_NAME} (期限: {task.DUE_DATE:yyyy/MM/dd})");
                    }
                }
            }
        }
        // 集計
        public void LogTaskCountByAssignee(List<TaskModel> tasks)
        {
            _logger.Info("担当者ごとの未完了タスク数集計を出力します。");

            // LINQ の GroupBy を用い、担当者ごとに現在抱えている未完了タスクの件数を集計
            var summary = tasks
                .Where(t => t.STATUS != "完了")
                .GroupBy(t => t.ASSIGNEE);

            foreach (var group in summary)
            {
                string assignee = group.Key;
                int count = group.Count();

                // NLog のログレベル（Info）で出力
                _logger.Info($"担当者: {assignee} / 未完了タスク数: {count}件");

                // コンソールにもリアルタイムで表示
                Console.WriteLine($"[INFO] 担当者: {assignee} / 未完了タスク数: {count}件");
            }
        }

    }
}
