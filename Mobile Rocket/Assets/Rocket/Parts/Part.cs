namespace Assets.Rocket.Parts
{
    public class Part
    {
        // If damage exceeds the health, destroy it
        public float DamageEndured;
        public float Health;
        public float Thrust;

        public void Damage(float damage)
        {
            DamageEndured += damage;

            if (DamageEndured > Health)
            {
                Destroy();
            }
        }

        public void Destroy()
        {

        }
    }
}
