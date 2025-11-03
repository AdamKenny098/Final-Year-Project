-> start
=== start ===
"Greetings adventurer! Care to browse my wares?"
+ ["Yes, show me what you have."] -> open_shop
+ ["Not right now."] -> decline

=== open_shop ===
"Quality Craftsmanship."
# OPENSHOP
-> post_shop

=== post_shop ===
"Did you find what you were looking for?"
+ ["Yes, thank you."] -> goodbye
+ ["Maybe later."] -> goodbye

=== decline ===
"Another time, perhaps."
-> goodbye

=== goodbye ===
# CLOSESHOP
"Safe travels, friend!"
-> END