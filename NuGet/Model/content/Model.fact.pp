namespace $rootnamespace$;

fact Identity {
key:
    string anonymousId;

query:
	Message* messages {
		Message m : m.recipient = this
	}

    DisableToastNotification* isToastNotificationDisabled {
        DisableToastNotification d : d.identity = this
            where not d.isReenabled
    }
}

fact Message {
key:
	unique;
	Identity sender;
	publish Identity recipient;
	string text;
}

fact DisableToastNotification {
key:
    unique;
    Identity identity;

query:
    bool isReenabled {
        exists EnableToastNotification e : e.disable = this
    }
}

fact EnableToastNotification {
key:
    DisableToastNotification* disable;
}
