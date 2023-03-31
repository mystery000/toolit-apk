namespace Toolit
{
    interface IRollback
    {
        void Transaction();
        void Rollback();
        void Commit();
    }
}
