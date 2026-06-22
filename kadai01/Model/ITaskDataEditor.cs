namespace kadai01.Model
{
    public interface ITaskDataEditor
    {

        // タスクの一覧を取得
        List<Tasks> GetTaskList(ListFilterMode mode);


        //// 指定したタスクを取得
        Tasks? GetTasks(int TASK_ID);


        //// コマンド(書き込み系)

        //// タスクの内容を更新する
        void Update(Tasks targetTask);
    }
}
