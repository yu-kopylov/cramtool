# Markup Language #

Tool treats each line of a word description as either a word form, an example or a translation depending on its first character.

| First Character | Line |
|:----------------|:-----|
| #               | word form |
| //              | example |
| everything else | translation |

# Example #

The description for the word **meagre** in the english-russian dictionary.

```
#meager
скудный
//a meagre diet of bread and water
//She supplements her meagre income by cleaning at night.
```

The article for this word would look like:

<table border='1'>
<tr><td>
  <b>meagre</b><br/>
  <b><i>meager</i></b><br/>
  скудный<br/>
  <br/>
  <i>Example: a meagre diet of bread and water</i><br/>
  <i>Example: She supplements her meagre income by cleaning at night.</i>
</td></tr>
</table>
