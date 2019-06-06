namespace SanguoshaServer.AI
{
    public class StanderAI : AIPackage
    {
        public StanderAI() : base("Stander")
        {
            StanderWeiAI wei = new StanderWeiAI();
            events.AddRange(wei.Events);
            use_cards.AddRange(wei.UseCards);

            StanderQunAI qun = new StanderQunAI();
            events.AddRange(qun.Events);
            use_cards.AddRange(qun.UseCards);

            StanderShuAI shu = new StanderShuAI();
            events.AddRange(shu.Events);
            use_cards.AddRange(shu.UseCards);

            StanderWuAI wu = new StanderWuAI();
            events.AddRange(wu.Events);
            use_cards.AddRange(wu.UseCards);
        }
    }
}
