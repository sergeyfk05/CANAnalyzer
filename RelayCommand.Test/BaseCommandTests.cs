using RelayCommands.Test.TestData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RelayCommands.Test
{
    public class BaseCommandTests
    {
        [Theory]
        [ClassData(typeof(CanExecuteChangeedEventTestData))]
        public async void CanExecuteChangedEventTest(BaseCommand command)
        {
            bool isInvoked = false;
            object sender = null;
            EventArgs arguments = null;

            command.CanExecuteChanged += (_sender, _args) =>
            {
                isInvoked = true;
                sender = _sender;
                arguments = _args;
            };
            command.RaiseCanExecuteChanged();

            await Task.Delay(10);

            Assert.True(isInvoked);
            Assert.Equal(command, sender);
        }

        [Theory]
        [ClassData(typeof(CanExecuteTestData))]
        public void CanExecuteTests(BaseCommand command, bool expectedResult, object parameter = null)
        {
            Assert.Equal(expectedResult, command.CanExecute(parameter));
        }
    }
}
