using System;
using System.Linq;
using LevelExtender.Logging;
using StardewModdingAPI;

namespace LevelExtender.Framework
{
    // from SkillPrestige by Alphablackwolf - https://github.com/Alphablackwolf/SkillPrestige
    /// <summary>Represents a command called in the SMAPI console interface.</summary>
    abstract class SMAPICommand<IMod>
    {
        /*********
        ** Fields
        *********/
        /// <summary>The name used to call the command in the console.</summary>
        private string Name { get; }

        /// <summary>The help description for the command.</summary>
        private string Description { get; }

        /// <summary>Whether the command is used only in test mode.</summary>
        private bool TestingCommand { get; }

        /// <summary>
        /// Mod (interface) instance
        /// </summary>
        protected IMod Mod { get; private set; }

        /*********
        ** Public methods
        *********/
        /// <summary>Register all loaded command types.</summary>
        /// <param name="helper">The SMAPI command helper.</param>
        /// <param name="testCommands">Whether to only register testing commands.</param>
        public static void RegisterCommands(IMod mod, ICommandHelper helper, bool testCommands)
        {
            var commandTypes = typeof(SMAPICommand<IMod>)
                .Assembly
                .GetTypesSafely()
                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(SMAPICommand<IMod>)));

            foreach (Type commandType in commandTypes)
            {
                SMAPICommand<IMod> command = (SMAPICommand<IMod>)Activator.CreateInstance(commandType);
                command.Mod = mod;
                if (!(testCommands ^ command.TestingCommand))
                    command.RegisterCommand(helper);
            }
        }


        /*********
        ** Protected methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name used to call the command in the console.</param>
        /// <param name="description">The help description for the command.</param>
        /// <param name="testingCommand">Whether the command is used only in test mode.</param>
        protected SMAPICommand(string name, string description, bool testingCommand = false)
        {
            this.Name = name;
            this.Description = description;
            this.TestingCommand = testingCommand;
        }

        /// <summary>Registers a command with the SMAPI console.</summary>
        private void RegisterCommand(ICommandHelper helper)
        {
            Logger.LogInformation($"Registering {this.Name} command...");
            helper.Add(this.Name, this.Description, (name, args) => this.Apply(args));
            Logger.LogInformation($"{this.Name} command registered.");
        }

        /// <summary>Applies the effect of a command when it is called from the console.</summary>
        protected abstract void Apply(string[] args);
    }
}
