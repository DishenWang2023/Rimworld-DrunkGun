using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Brvnch.DrunkGun
{
    // child of bullet class, which is child of projectile class
    public class Projectile_DrunkBullet : Bullet
    {
        #region Properties
        //
        public ThingDef_DrunkBullet Def
        {
            get
            {
                return this.def as ThingDef_DrunkBullet;
            }
        }
        #endregion Properties


        #region Overrides
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);

            /*
             * Null checking is very important in RimWorld.
             * 99% of errors reported are from NullReferenceExceptions (NREs).
             * Make sure your code checks if things actually exist, before they
             * try to use the code that belongs to said things.
             */
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn) //Fancy way to declare a variable inside an if statement. - Thanks Erdelf.
            {
                var rand = Rand.Value; // This is a random percentage between 0% and 100%
                if (rand <= Def.AddHediffChance) // If the percentage falls under the chance, success!
                {
                    /*
                     * Messages.Message flashes a message on the top of the screen.
                     * You may be familiar with this one when a colonist dies, because
                     * it makes a negative sound and mentioneds "So and so has died of _____".
                     *
                     * Here, we're using the "Translate" function. More on that later in
                     * the localization section.
                     */
                    string translatedMessage = TranslatorFormattedStringExtensions.Translate("Brvnch_DrunkBullet_SuccessMessage", launcher.Label, hitPawn.Label, Def.HediffToAdd.label);
                    Messages.Message(translatedMessage, MessageTypeDefOf.NegativeHealthEvent);

                    //This checks to see if the character has a heal differential, or hediff on them already.
                    var drunkOnPawn = hitPawn?.health?.hediffSet?.GetFirstHediffOfDef(Def.HediffToAdd);
                    var randomSeverity = Rand.Range(0.15f, 0.30f);
                    if (drunkOnPawn != null)
                    {
                        //If they already have drunk, add a random range to its severity.
                        //If severity reaches 1.0f, or 100%, drunk kills the target (or does it?).
                        drunkOnPawn.Severity += randomSeverity;
                    }
                    else
                    {
                        //These three lines create a new health differential or Hediff,
                        //put them on the character, and increase its severity by a random amount.
                        Hediff hediff = HediffMaker.MakeHediff(Def.HediffToAdd, hitPawn, null);
                        hediff.Severity = randomSeverity;
                        hitPawn.health.AddHediff(hediff, null, null);
                    }
                }
                else //failure!
                {
                    /*
                     * Motes handle all the smaller visual effects in RimWorld.
                     * Dust plumes, symbol bubbles, and text messages floating next to characters.
                     * This mote makes a small text message next to the character.
                     */
                    MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "Brvnch_DrunkBullet_FailureMote".Translate(Def.AddHediffChance), 12f);
                }
            }
        }
        #endregion Overrides
    }
}