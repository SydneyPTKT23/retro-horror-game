INCLUDE global_vars.ink

{ g_test_seen == true: -> repeat | -> start }

=== repeat ===

- You seem to have seen the test dialogue already... #narrator
 -> start

=== start ===
 
Once upon a time... #narrator

 + There were two choices. #speaker: player #emotion: neutral
    ++ But one of the choices had another choice nested within... #speaker: player #emotion: neutral
        -> example_choice(true)
    ++ How could I choose? #speaker: player #emotion: neutral
        -> start
 + There were some lines of content. #speaker: player #emotion: neutral
    -> example_choice(false)


=== example_choice(example) ===
~ g_example = example
- They lived happily ever after. #narrator #function: TestFunction
~ g_test_seen = true
- Example result: {example}. #narrator

    -> END