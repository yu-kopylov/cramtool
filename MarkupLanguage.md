# Markup Language #

Tool treats each line of word description as either word form, example or translation depending on its first caracter.

| First Character | Line |
|:----------------|:-----|
| #               | word form |
| //              | example |
| everything else | translation |

# Example #

Description for word **meagre** in english-russian dictionary.

```
#meager
скудный
//a meagre diet of bread and water
//She supplements her meagre income by cleaning at night.
```

Article for this word would look like:

<table border='1'><tr><td>
<b>meagre</b>

<b><i>meager</i></b>

скудный<br>
<br>
<i>Example: a meagre diet of bread and water</i>

<i>Example: She supplements her meagre income by cleaning at night.</i>
</td></tr></table>