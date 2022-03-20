// ReSharper disable All

using System;

namespace CSharp
{
    public class Monad
    {
        public Either<int, Error> SafeStringToInt(string value) =>
            int.TryParse(value, out int i)
                ? Either<int, Error>.Left(i)
                : Either<int, Error>.Right(Error.NotInteger);
        
        public string IntToString(int i) => i.ToString();
        
        public Either<int, Error> SafeIsPositive(int value) =>
            value > 0 
                ? Either<int, Error>.Left(value) 
                : Either<int, Error>.Right(Error.NotPositive);
        
        public enum Error { NotInteger, NotPositive }
        
        public Either<TLeftOut, TRight> MapLeft<TLeftIn, TLeftOut, TRight>(
            Either<TLeftIn, TRight> either, Func<TLeftIn, TLeftOut> morphism) =>
            either.Match(
                left => Either<TLeftOut, TRight>.Left(morphism(left)), 
                right => Either<TLeftOut, TRight>.Right(right));

        public Either<TLeft, TRight> JoinLeft<TLeft, TRight>(Either<Either<TLeft, TRight>, TRight> either) =>
            either.Match(
                left => left, 
                right => Either<TLeft, TRight>.Right(right));

        public Either<TLeftOut, TRight> BindLeft<TLeftIn, TLeftOut, TRight>(
            Either<TLeftIn, TRight> either, Func<TLeftIn, Either<TLeftOut, TRight>> morphism) =>
            JoinLeft(MapLeft(either, morphism));
        
        public Either<string, Error> SafeStringIsPositiveString(string value) => 
            MapLeft(BindLeft(SafeStringToInt(value), SafeIsPositive), IntToString);

        // Fluent variation : SafeStringToInt(value).BindLeft(SafeIsPositive).MapLeft(IntToString);
    }
}