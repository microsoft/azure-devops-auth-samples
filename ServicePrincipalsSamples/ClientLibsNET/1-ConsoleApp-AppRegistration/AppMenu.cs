using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServicePrincipalsSamples
{
    public class AppMenu
    {
        private static int optionsCounter;

        private readonly List<OptionGroup> optionGroups = new();
        private readonly List<Option> allOptions = new();

        private string menuString;

        public async Task<bool> DisplayMenu()
        {
            menuString ??= GenerateMenuAsString();

            Console.Write(menuString);

            try
            {
                int optionNumber = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();

                if (optionNumber == 0)
                {
                    return false;
                }

                var option = allOptions[optionNumber - 1];
                await option.Operation();
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentOutOfRangeException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid option. Please select one of the options above.");
                Console.ResetColor();
            }

            return true;
        }

        public OptionGroup AddOptionGroup(string name)
        {
            var group = new OptionGroup(name);
            optionGroups.Add(group);
            return group;
        }

        private string GenerateMenuAsString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("\n----------------------------------------------------");
            stringBuilder.AppendLine("MENU OPTIONS:\n");
            stringBuilder.AppendLine($"0) Exit");

            foreach (var group in optionGroups)
            {
                stringBuilder.AppendLine($"\n{group.Name}:");

                foreach (var option in group.Options)
                {
                    optionsCounter++;
                    allOptions.Add(option);
                    stringBuilder.AppendLine($"{optionsCounter}) {option.Name}");
                }
            }

            stringBuilder.AppendLine("----------------------------------------------------");
            stringBuilder.Append("\nChoose an option: ");

            return stringBuilder.ToString();
        }

        public class OptionGroup
        {
            public string Name { get; }
            public List<Option> Options { get; } = new List<Option>();

            public OptionGroup(string name)
            {
                Name = name;
            }

            public OptionGroup AddOption(string name, Func<Task> operation)
            {
                Options.Add(new Option(name, operation));
                return this;
            }
        }

        public class Option
        {
            public string Name { get; }
            public Func<Task> Operation { get; }

            public Option(string name, Func<Task> operation)
            {
                Name = name;
                Operation = operation;
            }
        }
    }
}
