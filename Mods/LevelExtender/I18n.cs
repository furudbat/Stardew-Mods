using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

namespace LevelExtender
{
    /// <summary>Get translations from the mod's <c>i18n</c> folder.</summary>
    /// <remarks>This is auto-generated from the <c>i18n/default.json</c> file when the T4 template is saved.</remarks>
    [GeneratedCode("TextTemplatingFileGenerator", "1.0.0")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Deliberately named for consistency and to match translation conventions.")]
    internal static class I18n
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod's translation helper.</summary>
        private static ITranslationHelper Translations;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="translations">The mod's translation helper.</param>
        public static void Init(ITranslationHelper translations)
        {
            I18n.Translations = translations;
        }

        /// <summary>Get a translation equivalent to "Level Up".</summary>
        public static string LevelUp()
        {
            return I18n.GetByKey("LevelUp");
        }

        /// <summary>Get a translation equivalent to "Your {{skillName}} level allowed you to obtain extra {{itemName}}(s)!".</summary>
        /// <param name="skillName">The value to inject for the <c>{{skillName}}</c> token.</param>
        /// <param name="itemName">The value to inject for the <c>{{itemName}}</c> token.</param>
        public static string ExtraItemMessage(object skillName, object itemName)
        {
            return I18n.GetByKey("ExtraItemMessage", new { skillName, itemName });
        }

        /// <summary>Get a translation equivalent to "Your {{skillName}} level allowed you to obtain {{extraItemAmount}} extra {{itemName}}!".</summary>
        /// <param name="skillName">The value to inject for the <c>{{skillName}}</c> token.</param>
        /// <param name="extraItemAmount">The value to inject for the <c>{{extraItemAmount}}</c> token.</param>
        /// <param name="itemName">The value to inject for the <c>{{itemName}}</c> token.</param>
        public static string ExtraItemMessageWithAmount(object skillName, object extraItemAmount, object itemName)
        {
            return I18n.GetByKey("ExtraItemMessageWithAmount", new { skillName, extraItemAmount, itemName });
        }

        /// <summary>Get a translation equivalent to "{{amount}}k".</summary>
        /// <param name="amount">The value to inject for the <c>{{amount}}</c> token.</param>
        public static string KAmountWithUnit(object amount)
        {
            return I18n.GetByKey("KAmountWithUnit", new { amount });
        }

        /// <summary>Get a translation equivalent to "~{{amount}}".</summary>
        /// <param name="amount">The value to inject for the <c>{{amount}}</c> token.</param>
        public static string XPUntilNextLevel(object amount)
        {
            return I18n.GetByKey("XPUntilNextLevel", new { amount });
        }

        /// <summary>Get a translation equivalent to "Exp {{amount}}".</summary>
        /// <param name="amount">The value to inject for the <c>{{amount}}</c> token.</param>
        public static string Exp(object amount)
        {
            return I18n.GetByKey("Exp", new { amount });
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get a translation by its key.</summary>
        /// <param name="key">The translation key.</param>
        /// <param name="tokens">An object containing token key/value pairs. This can be an anonymous object (like <c>new { value = 42, name = "Cranberries" }</c>), a dictionary, or a class instance.</param>
        private static Translation GetByKey(string key, object tokens = null)
        {
            if (I18n.Translations == null)
                throw new InvalidOperationException($"You must call {nameof(I18n)}.{nameof(I18n.Init)} from the mod's entry method before reading translations.");
            return I18n.Translations.Get(key, tokens);
        }
    }
}
