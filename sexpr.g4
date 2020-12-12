grammar sexpr;

sexpr
   : list EOF
   ;

item
   : atom
   | list
   ;

list
   : LPAREN atom item* RPAREN
   ;

atom
   : STRING
   | SYMBOL
   | NUMBER
   | DOT
   ;

STRING
   : '"' ('\\' . | ~ ('\\' | '"'))* '"'
   ;

WHITESPACE
   : (' ' | '\n' | '\t' | '\r')+ -> skip
   ;

NUMBER
   : ('+' | '-')? (DIGIT)+ ('.' (DIGIT)+)?
   ;

SYMBOL
   : SYMBOL_START (SYMBOL_START | DIGIT)*
   ;

LPAREN
   : '('
   ;

RPAREN
   : ')'
   ;

DOT
   : '.'
   ;

fragment SYMBOL_START
   : ('a' .. 'z')
   | ('A' .. 'Z')
   | '+'
   | '-'
   | '*'
   | '/'
   | '.'
   ;

fragment DIGIT
   : ('0' .. '9')
   ;

