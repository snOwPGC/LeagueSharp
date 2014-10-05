using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.CompilerServices;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SKO_Galio
{
    class Program
    {
        private const string ChampionName = "Galio";

        private static Menu Config;

        private static Orbwalking.Orbwalker Orbwalker;

        private static List<Spell> SpellList = new List<Spell>();

        private static Spell Q, W, E, R;

        private static Items.Item DFG, HDR, BKR, BWC, YOU;

        private static Obj_AI_Hero Player;

        private static SpellSlot IgniteSlot;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad; 
        }

        private static void OnGameLoad(EventArgs args) 
        {
            Player = ObjectManager.Player;
            if (Player.BaseSkinName != ChampionName) return;

            SKOUpdater.InitializeSKOUpdate();

            Q = new Spell(SpellSlot.Q, 940f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 1180f);
            R = new Spell(SpellSlot.R, 560f);

            Q.SetSkillshot(0.5f, 120, 1300, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 140, 1200, false, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 300, 0, false, SkillshotType.SkillshotCircle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            HDR = new Items.Item(3074, Player.AttackRange+50);
            BKR = new Items.Item(3153, 450f);
            BWC = new Items.Item(3144, 450f);
            YOU = new Items.Item(3142, 185f);
            DFG = new Items.Item(3128, 750f);

            IgniteSlot = Player.GetSpellSlot("SummonerDot");

            //SKO Galio
            Config = new Menu(ChampionName, "SKOGalio", true);

            //TargetSelector
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            //Orbwalker
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQCombo", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("MinEnemys", "Min enemys for R")).SetValue(new Slider(3, 5, 1));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(Config.Item("Orbwalk").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Extra
            Config.AddSubMenu(new Menu("Extra", "Extra"));
            Config.SubMenu("Extra").AddItem(new MenuItem("AutoShield", "Auto Shield")).SetValue(true);



            //Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q")).SetValue(true);
            Config.SubMenu("Harass").AddItem(new MenuItem("UseEHarass", "Use E")).SetValue(true);
            Config.SubMenu("Harass").AddItem(new MenuItem("ActiveHarass", "Harass key").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

            //Farm
            Config.AddSubMenu(new Menu("Lane Clear", "Lane"));
            Config.SubMenu("Lane").AddItem(new MenuItem("UseQLane", "Use Q")).SetValue(true);
            Config.SubMenu("Lane").AddItem(new MenuItem("UseELane", "Use E")).SetValue(true);
            Config.SubMenu("Lane").AddItem(new MenuItem("ActiveLane", "Lane Key").SetValue(new KeyBind(Config.Item("LaneClear").GetValue<KeyBind>().Key, KeyBindType.Press)));

            //Kill Steal
            Config.AddSubMenu(new Menu("KillSteal", "Ks"));
            Config.SubMenu("Ks").AddItem(new MenuItem("ActiveKs", "Use KillSteal")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseQKs", "Use Q")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseEKs", "Use E")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseIgnite", "Use Ignite")).SetValue(true);


            //Drawings
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "Draw R")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "Lag Free Circles").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleQuality", "Circles Quality").SetValue(new Slider(100, 100, 10)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleThickness", "Circles Thickness").SetValue(new Slider(1, 10, 1)));

            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;


        }

        private static void OnGameUpdate(EventArgs args)
        {
            Orbwalker.SetAttacks(true);
            //Orbwalker.SetMovement(true);

            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active) {
                Combo();
            }
            if (Config.Item("ActiveHarass").GetValue<KeyBind>().Active)
            {
                Harass();
            }
            if (Config.Item("ActiveKs").GetValue<bool>())
            {
                KillSteal();
            }
            if (Config.Item("ActiveFarm").GetValue<KeyBind>().Active)
            {
                Farm();
            }
        }

        private static void Combo() {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (!Player.HasBuff("GalioIdolOfDurand")) {
                Orbwalker.SetMovement(true);
            }

            if (target != null) 
            {
                if (Q.IsReady() && Player.Distance(target) <= Q.Range && Config.Item("UseQCombo").GetValue<bool>())
                {
                    Q.Cast(target);
                }
                else if (E.IsReady() && Player.Distance(target) <= E.Range && Config.Item("UseECombo").GetValue<bool>())
                {
                    E.Cast(target);
                }else if (R.IsReady() && GetEnemys(target) >= Config.Item("MinEnemys").GetValue<Slider>().Value && Config.Item("UseRCombo").GetValue<bool>())
                {
                    Orbwalker.SetMovement(false);
                    R.Cast(target, false, true);
                    if (Config.Item("UseWCombo").GetValue<bool>())
                    {
                        W.Cast(Player);
                    }
                }
            
            }
        }

        private static void Harass(){
        var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (target != null){
                if (Q.IsReady() && Player.Distance(target) <= Q.Range && Config.Item("UseQHarass").GetValue<bool>())
                {
                    Q.Cast(target);
                }
                else if (E.IsReady() && Player.Distance(target) <= E.Range && Config.Item("UseEHarass").GetValue<bool>())
                {
                    E.Cast(target);
                }
            }
        }

        private static void Farm() {
            var AllMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

            
                foreach(var minion in AllMinions){

                    if (Config.Item("UseQLane").GetValue<bool>() && Q.IsReady() && Player.Distance(minion) <= Q.Range) {
                        Q.Cast(minion);
                    }
                    if (Config.Item("UseELane").GetValue<bool>() && E.IsReady() && Player.Distance(minion) <= E.Range)
                    {
                        E.Cast(minion);
                    }
                }
            

        }

        private static void KillSteal() {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            var IgniteDmg = Damage.GetSummonerSpellDamage(Player, target,Damage.SummonerSpell.Ignite);
            var QDmg = Damage.GetSpellDamage(Player, target, SpellSlot.Q);
            var EDmg = Damage.GetSpellDamage(Player, target, SpellSlot.E);

            if (target != null)
            {
                if (Config.Item("UseIgnite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
                Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
                {
                    if (IgniteDmg > target.Health)
                    {
                        Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                    }
                }

                if (Config.Item("UseQKs").GetValue<bool>() && Q.IsReady()) {
                    if (QDmg >= target.Health) {
                        Q.Cast(target);
                    }
                }
                if (Config.Item("UseEKs").GetValue<bool>() && E.IsReady())
                {
                    if (EDmg >= target.Health)
                    {
                        E.Cast(target);
                    }
                }
            }
           
        }

        private static int GetEnemys(Obj_AI_Hero target) {
            int Enemys = 0;
            foreach(Obj_AI_Hero enemys in ObjectManager.Get<Obj_AI_Hero>()){

                var pred = R.GetPrediction(enemys, true);
                if(pred.Hitchance >= HitChance.High && !enemys.IsMe && enemys.IsEnemy && Vector3.Distance(Player.Position, pred.UnitPosition) <= R.Range){
                    Enemys = Enemys + 1;
                }
            }
        return Enemys;
        }

        private static void OnDraw(EventArgs args)
        {
            if (Config.Item("CircleLag").GetValue<bool>())
            {
                if (Config.Item("DrawQ").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value,
                        Config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (Config.Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value,
                        Config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value,
                        Config.Item("CircleQuality").GetValue<Slider>().Value);
                }
            }
            else
            {
                if (Config.Item("DrawQ").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.White);
                }
                if (Config.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White);
                }
                if (Config.Item("DrawR").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.White);
                }

            }
        }
    }
}
