public interface IDamagable
{
    void TakeDamage(float damage, int viewID);
    void RPC_TakeDamage(int viewID, float damage);
}
