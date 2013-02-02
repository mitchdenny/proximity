using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Networking.Proximity;

namespace Proximity
{
    public class MainViewModel
    {
        public MainViewModel(Dispatcher dispatcher)
        {
            Events = new ObservableCollection<NfcEvent>();
            InitializeProximityDevice();
            Dispatcher = dispatcher;
        }

        public Dispatcher Dispatcher { get; set; }

        private void InitializeProximityDevice()
        {
            Device = ProximityDevice.GetDefault();
            Device.DeviceArrived += Device_DeviceArrived;
            Device.DeviceDeparted += Device_DeviceDeparted;

            Device.SubscribeForMessage("WindowsUri", ReceiveMessage);
            Device.SubscribeForMessage("WindowsMime", ReceiveMessage);
            Device.SubscribeForMessage("NDEF", ReceiveMessage);
        }

        private void ReceiveMessage(ProximityDevice device, ProximityMessage message)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    var @event = new MessageReceivedNfcEvent();
                    Events.Add(@event);
                }
                );
        }

        private void Device_DeviceArrived(ProximityDevice sender)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var @event = new ArrivedNfcEvent();
                Events.Add(@event);
            }
                           );
        }

        private void Device_DeviceDeparted(ProximityDevice sender)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var @event = new DepartedNfcEvent();
                Events.Add(@event);
            }
                           );
        }

        public ProximityDevice Device { get; private set; }
        public ObservableCollection<NfcEvent> Events { get; set; }
    }

    public abstract class NfcEvent
    {
        public NfcEvent()
        {
            TimeStamp = DateTimeOffset.Now;
        }

        public DateTimeOffset TimeStamp { get; private set; }
        public abstract string Description { get; }
    }

    public class ArrivedNfcEvent : NfcEvent
    {
        public override string Description
        {
            get { return "NFC Tag Arrived"; }
        }
    }

    public class DepartedNfcEvent : NfcEvent
    {
        public override string Description
        {
            get { return "NFC Tag Departed"; }
        }
    }

    public class MessageReceivedNfcEvent : NfcEvent
    {
        public override string Description
        {
            get { return "NFC Message Received"; }
        }
    }
}
