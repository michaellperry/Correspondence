namespace $rootnamespace$;

fact Identity {
key:
    string anonymousId;

query:
	MessageBoard *messageBoards {
		Share s : s.identity = this
		MessageBoard m : m = s.messageBoard
	}

    EnableToastNotification* isToastNotificationEnabled {
        EnableToastNotification e : e.identity = this
            where not d.isDisabled
    }
}

fact Share {
key:
	publish Identity identity;
	MessageBoard messageBoard;
}

fact MessageBoard {
key:
	string identifier;

query:
	Message* messages {
		Message m : m.Forum = this
	}
}

fact Domain {
key:
}

fact Message {
key:
	unique;
	publish Forum forum;
	publish Domain domain;
	string text;
}

fact EnableToastNotification {
key:
    unique;
    Identity identity;

query:
    bool isDisabled {
        exists DisableToastNotification d : d.enable = this
    }
}

fact DisableToastNotification {
key:
    EnableToastNotification* enable;
}
