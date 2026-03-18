-> start

=== start ===
What do you want?

+ [Any work available?] -> open_quests
+ [Nothing right now.] -> decline


=== open_quests ===
I do have some jobs for you.
# OPENQUESTS
-> quest_wait


=== quest_wait ===
Take your time.
+ [Continue] -> goodbye


=== decline ===
Very well.
-> goodbye


=== goodbye ===
Farewell.
-> END