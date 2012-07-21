namespace $rootnamespace$;

fact Individual {
key:
    string anonymousId;

query:
	MessageBoard *messageBoards {
		Share s : s.individual = this
		MessageBoard m : m = s.messageBoard
	}

    EnableToastNotification* isToastNotificationEnabled {
        EnableToastNotification e : e.individual = this
            where not e.isDisabled
    }
}

fact Share {
key:
	publish Individual individual;
	MessageBoard messageBoard;
}

fact MessageBoard {
key:
	string topic;

query:
	Message* messages {
		Message m : m.messageBoard = this
	}
}

fact Domain {
key:
}

fact Message {
key:
	unique;
	publish MessageBoard messageBoard;
	publish Domain domain;
	string text;
}

fact EnableToastNotification {
key:
    unique;
    Individual individual;

query:
    bool isDisabled {
        exists DisableToastNotification d : d.enable = this
    }
}

fact DisableToastNotification {
key:
    EnableToastNotification* enable;
}
