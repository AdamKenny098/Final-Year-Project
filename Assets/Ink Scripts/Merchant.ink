-> start

VAR tradeOutcome = ""

=== start ===
"Greetings adventurer! Care to browse my wares?"
+ ["Yes, show me what you have."] -> open_shop
+ ["Not right now."] -> decline


=== open_shop ===
"Quality Craftsmanship."
# OPENSHOP
-> shop_wait


=== shop_wait ===
"Take your time."
+ [continue] -> post_shop


=== post_shop ===
{
- tradeOutcome == "barter_success":
    "Ahh, you drive a hard bargain. Very well."
- tradeOutcome == "barter_refused":
    "No — these prices are fair. Real business only."
- tradeOutcome == "steal_success":
    "Strange… I thought something was there a moment ago."
- tradeOutcome == "steal_caught":
    "I will not tolerate thieves. We are done here."
- else:
    "Did you find what you were looking for?"
}

+ [Continue] -> post_shop_exit


=== decline ===
"Another time, perhaps."
-> goodbye

=== post_shop_exit ===
{
- tradeOutcome == "barter_success":
    -> goodbye_barter_success
- tradeOutcome == "barter_refused":
    -> goodbye_barter_fail
- tradeOutcome == "steal_success":
    -> goodbye_steal_success
- tradeOutcome == "steal_caught":
    -> goodbye_steal_fail
- else:
    -> goodbye
}

=== goodbye ===
~ tradeOutcome = ""
"Safe travels, friend!"
-> END

=== goodbye_barter_success ===
"Safe travels, friend."
~ tradeOutcome = ""
-> END

=== goodbye_barter_fail===
"Perhaps we’ll do business another time."
~ tradeOutcome = ""
-> END

=== goodbye_steal_success ===
"Safe travels, friend."
~ tradeOutcome = ""
-> END

=== goodbye_steal_fail===
"Get out of my sight"
~ tradeOutcome = ""
-> END


