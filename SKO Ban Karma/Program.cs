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

namespace SKO_Ban_Karma
{
    class Program
    {
        //TEM QUE BANIR KARMAR CARALHO

        private const string ChampionName = "Karma"; //OP

        private static List<Spell> SpellList = new List<Spell>();

        private static Spell Q;

        private static Spell W;

        private static Spell E;

        private static Spell R;

        private static Menu Config;

        private static Orbwalking.Orbwalker Orbwalker;

        private static Obj_AI_Hero Player;

        private static Items.Item HDR;

        private static Items.Item BKR;

        private static Items.Item BWC;

        private static Items.Item YOU;

        private static Items.Item DFG;

        private static SpellSlot IgniteSlot;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args) 
        {
            Player = ObjectManager.Player;

            if (Player.BaseSkinName != ChampionName) return;

            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 654f);
            E = new Spell(SpellSlot.E, 800f);
            R = new Spell(SpellSlot.R, 0f);

            Q.SetSkillshot(0.25f, 70, 1800, true, SkillshotType.SkillshotLine);


            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            BKR = new Items.Item(3153, 450f);
            BWC = new Items.Item(3144, 450f);
            YOU = new Items.Item(3142, 185f);
            DFG = new Items.Item(3128, 750f);

            IgniteSlot = Player.GetSpellSlot("SummonetDot");

            //SKO SKO Ban Karma
            Config = new Menu(ChampionName, "SKOBanKarma", true);

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
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            //Harass
            Config.AddSubMenu(new Menu("Harass", "Harass"));
            Config.SubMenu("Harass").AddItem(new MenuItem("UseQHarass", "Use Q")).SetValue(true);
            Config.SubMenu("Harass").AddItem(new MenuItem("UseWHarass", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRHarass", "Use R")).SetValue(true);
            Config.SubMenu("Harass").AddItem(new MenuItem("ActiveHarass", "Harass key").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));

            //Farm
            Config.AddSubMenu(new Menu("Lane Clear", "Lane"));
            Config.SubMenu("Lane").AddItem(new MenuItem("UseQLane", "Use Q")).SetValue(true);

            Config.SubMenu("Lane").AddItem(new MenuItem("ActiveLane", "Lane Key").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            //Kill Steal
            Config.AddSubMenu(new Menu("KillSteal", "Ks"));
            Config.SubMenu("Ks").AddItem(new MenuItem("ActiveKs", "Use KillSteal")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseQKs", "Use Q")).SetValue(true);
            Config.SubMenu("Ks").AddItem(new MenuItem("UseIgnite", "Use Ignite")).SetValue(true);


            //Drawings
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawQ", "Draw Q")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "Draw W")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleLag", "Lag Free Circles").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleQuality", "Circles Quality").SetValue(new Slider(100, 100, 10)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleThickness", "Circles Thickness").SetValue(new Slider(1, 10, 1)));

            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;

            Game.PrintChat("<font color='#1d87f2'>SKO Ban Karma Loaded!</font>");
        }

        private static void OnGameUpdate(EventArgs args) 
        {
            if (Config.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (Config.Item("ActiveHarass").GetValue<KeyBind>().Active)
            {
                Harass();
            }
            if (Config.Item("ActiveLane").GetValue<KeyBind>().Active)
            {
                Farm();
            }
            if (Config.Item("ActiveKs").GetValue<bool>())
            {
                KillSteal();
            }
            AutoW();
        }

        private static void Combo() {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);

            if (Config.Item("UseItems").GetValue<bool>())
            {
                BKR.Cast(target);
                YOU.Cast();
                BWC.Cast(target);
                DFG.Cast(target);
            }

            if(W.IsReady() && Player.Distance(target) <= W.Range && Config.Item("UseWCombo").GetValue<bool>()){
                W.Cast(target);
            }
            if (Q.IsReady() && Player.Distance(target) <= Q.Range && Config.Item("UseQCombo").GetValue<bool>()) 
            {
                if (Config.Item("UseRCombo").GetValue<bool>() && Player.HasBuff("KarmaMantra") && !Q.Collision)
                {
                    R.Cast();
                    Q.Cast(target);
                }
                else {
                    R.Cast();
                    Q.Cast(target);
                }
            }
            if (E.IsReady() && Config.Item("UseECombo").GetValue<bool>() && Player.Distance(target) > W.Range && !Player.HasBuff("KarmaMantra")) {
 
                    E.Cast(Player);
            }

        }
        private static void AutoW() {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            //Auto W
            if (Player.Health <= Player.MaxHealth * 0.70 && Player.HasBuff("KarmaMantra") && Config.Item("UseWCombo").GetValue<bool>())
            {
                if (W.IsReady() && Player.Distance(target) <= W.Range)
                {
                    R.Cast();
                    W.Cast(target);

                }
            }
        }
        private static void Harass()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (W.IsReady() && Player.Distance(target) <= W.Range && Config.Item("UseWHarass").GetValue<bool>())
            {
                W.Cast(target);
            }
            if (Q.IsReady() && Player.Distance(target) <= Q.Range)
            {
                if (Config.Item("UseRHarass").GetValue<bool>() && Player.HasBuff("KarmaMantra"))
                {
                    R.Cast();
                    Q.Cast(target);
                }
                else
                {
                    R.Cast();
                    Q.Cast(target);
                }
            }
        }
        private static void KillSteal()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            var igniteDmg = Damage.GetSummonerSpellDamage(Player, target,Damage.SummonerSpell.Ignite);
            var QDmg = Damage.GetSpellDamage(Player, target, SpellSlot.Q);
            if (target != null && Config.Item("UseIgnite").GetValue<bool>() && IgniteSlot != SpellSlot.Unknown &&
            Player.SummonerSpellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (igniteDmg > target.Health)
                {
                    Player.SummonerSpellbook.CastSpell(IgniteSlot, target);
                }
            }
            
            if (Config.Item("UseQKs").GetValue<bool>() && target != null && Q.IsReady() && Player.Distance(target) <= Q.Range) {
                if (target.Health < QDmg) {
                    Q.Cast(target);
                }
            }

        }
        private static void Farm()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            var allminions = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.Health);

            if (Config.Item("UseQLane").GetValue<bool>())
            {
                foreach (var minion in allminions)
                {
                    if (Q.IsReady() && minion.IsValidTarget() && Player.Distance(minion) <= Q.Range)
                    {
                        Q.Cast(minion);
                    }

                }
            }
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
                if (Config.Item("DrawW").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White,
                        Config.Item("CircleThickness").GetValue<Slider>().Value,
                        Config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (Config.Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White,
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
                if (Config.Item("DrawW").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.White);
                }
                if (Config.Item("DrawE").GetValue<bool>())
                {
                    Drawing.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.White);
                }

            }
        }
    }
}
