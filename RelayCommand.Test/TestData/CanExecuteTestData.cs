using System.Collections;
using System.Collections.Generic;

namespace RelayCommands.Test.TestData
{
    public class CanExecuteTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new RelayCommand(() => { }, () => { return false; }), false, null };
            yield return new object[] { new RelayCommandAsync(() => { }, () => { return false; }), false, null };
            yield return new object[] { new RelayCommandWithParameter<object>((o) => { }, () => { return false; }), false, null };
            yield return new object[] { new RelayCommandWithParameterAsync<object>((o) => { }, () => { return false; }), false, null };
            yield return new object[] { new RelayCommand(() => { }, () => { return true; }), true, null };
            yield return new object[] { new RelayCommandAsync(() => { }, () => { return true; }), true, null };
            yield return new object[] { new RelayCommandWithParameter<object>((o) => { }, () => { return true; }), true, null };
            yield return new object[] { new RelayCommandWithParameterAsync<object>((o) => { }, () => { return true; }), true, null };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
