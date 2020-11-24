using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace RelayCommands.Test.TestData
{
    public class CanExecuteChangeedEventTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new RelayCommand(() => { }, () => { return false; })};
            yield return new object[] { new RelayCommandAsync(() => { }, () => { return false; })};
            yield return new object[] { new RelayCommandWithParameter<object>((o) => { }, () => { return false; })};
            yield return new object[] { new RelayCommandWithParameterAsync<object>((o) => { }, () => { return false; })};
            yield return new object[] { new RelayCommand(() => { }, () => { return true; })};
            yield return new object[] { new RelayCommandAsync(() => { }, () => { return true; })};
            yield return new object[] { new RelayCommandWithParameter<object>((o) => { }, () => { return true; })};
            yield return new object[] { new RelayCommandWithParameterAsync<object>((o) => { }, () => { return true; })};
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
