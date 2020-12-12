# ZhangShashaCSharp
An implementation of Zhang/Shasha in C#, including a list of operations.

This is a basic implemementation of the Zhang/Shasha algorithm for
tree diffs. It is, so far, the only diff algorithm that makes good sense,
because the paper is precise and easy to implement. I have some
concerns about general trees, because the paper examples are
with binary trees. The paper does not include the operations that need to
be performed; this implementation does.

This implementation is based on the [Alex Ilchenko's](https://github.com/ijkilchenko)
[implementation](https://github.com/ijkilchenko/ZhangShasha). It is the only
one that is a faithful, exact implementation of the psuedo-code listed in
the [Zhang/Shasha](https://epubs.siam.org/doi/abs/10.1137/0218082) paper.