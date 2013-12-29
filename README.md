Correspondence
==============

The Correspondence Collaboration Framework caches data on the mobile device, and queues changes while offline. When
a connection becomes available, it synchronizes changes with a central distributor. While online, it updates the
user interface in real time as other users make changes.

## Set up a distributor
Go to http://correspondencecloud.com to set up a Correspondence distributor. This is how devices running your app will collaborate with each other. You can use our distributor, or deploy your own to IIS or Azure.

## Download using NuGet
Create a portable class library for your model. Add the package:
* Correspondence.Model

Create another portable class libraries for your domain services, view models, and other shared code. Add the package:
* Correspondence.Core

Add the app package that's right for your application:
* Windows Store, Windows Phone, WPF, or Silverlight - Correspondence.App
* MVC - Correspondence.Web.App

Create a unit test library and add the package:
* Correspondence.UnitTest

See the Readme.txt file for additional instructions.

### Step 1: define a fact
You express your model in a language called Factual. A fact looks like this:
<pre>
fact Message {
key:
    Conversation conversation;
    User sender;

    string body;
}
</pre>

### Step 2: add a fact to the community
Adding a fact stores it in the local database and publishes it for other peers.
<pre>
public partial class Conversation
{
    public async Task SendMessageAsync(User sender, string body)
    {
        await Community.AddFactAsync(new Message(this, sender, body));
    }
}
</pre>

### Step 3: query for related facts
A query is expressed in Factual as part of a fact. The colon (:) is pronounced "such that", as in "messages is the set of Message facts m *such that* m.conversation is this Conversation".
<pre>
fact Conversation {
    // ...

query:
    Message *messages {
        Message m : m.conversation = this
    }
}
</pre>

### Step 4: access query results
Query results appear as an enumerable property of the fact. The results are bindable, even through a view model:
<pre>
public class ConversationViewModel
{
    private Conversation _conversation;

    public ConversationViewModel(Conversation conversation)
    {
        _conversation = conversation;
    }

    public IEnumerable<string> Messages
    {
        get
        {
            return _conversation.Messages
                .Select(m => string.Format("{0}: {1}",
                    m.Sender.UserName,
                    m.Body));
        }
    }
}
</pre>
