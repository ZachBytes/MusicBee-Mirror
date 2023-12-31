# MusicBee-Mirror
This plugin allows one instance of MusicBee (the server) to mirror another remote instance of MusicBee (the client) on the same network.

The goal of this plugin is to allow an instance of MusicBee running from a portable Windows device, like a Surface, handheld, or laptop (your remote / client) to control your main MusicBee library on the Windows device that is hooked up to your speakers (usually your home PC / server).

This effectively turns any Windows 10 device into a MusicBee remote, using MusicBee's native GUI. The library files only need to live on the server machine, the client machine does not need them. The client MusicBee instance will be sending messages to the server MusicBee instance which will play your music locally, so there is no loss in sound quality. 

The only caveat is the exclamation marks for "Missing Track" will show up on the client MusicBee library. You can just ignore these, as they are not relevant to the client, only to the server. 
