using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace CastleIoC
{
    public class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer();

            container.Register(Component.For<Shopper>());
            container.Register(Component.For<ICreditCard>().ImplementedBy<MasterCard>().Named("default"));
        }

        public class Resolver
        {
            private Dictionary<Type, Type> dependencyMap = new Dictionary<Type, Type>();

            public T Resolve<T>()
            {
                return (T)Resolve(typeof(T));
            }

            public void Register<TFrom, TTo>()
            {
                dependencyMap.Add(typeof(TFrom), typeof(TTo));
            }

            private object Resolve(Type typeToResolve)
            {
                Type resolvedType = null;

                try
                {
                    resolvedType = dependencyMap[typeToResolve];
                }
                catch
                {
                    throw new Exception(string.Format("Could not resolve type {0}", typeToResolve.FullName));
                }

                var firstContructor = resolvedType.GetConstructors().First();
                var contructorParameters = firstContructor.GetParameters();

                if (contructorParameters.Count() == 0)
                    return Activator.CreateInstance(resolvedType);

                IList<object> parameters = new List<object>();
                foreach (var parameterToResolve in contructorParameters)
                {
                    parameters.Add(Resolve(parameterToResolve.ParameterType));
                }
                return firstContructor.Invoke(parameters.ToArray());
            }
        }

        public class Visa : ICreditCard
        {
            public int ChargeCount { get { return 0; } }

            public string Charge()
            {
                return "Charging the Visa!";
            }
        }

        public class MasterCard : ICreditCard
        {
            public int ChargeCount { get; set; }

            public string Charge()
            {
                ChargeCount++;
                return "Swiping the MasterCard";
            }
        }

        public interface ICreditCard
        {
            string Charge();
            int ChargeCount { get; }
        }

        public class Shopper
        {
            private readonly ICreditCard _creditCard;

            public Shopper(ICreditCard creditCard)
            {
                this._creditCard = creditCard;
            }

            public int ChargesForCurrentCard
            {
                get { return this._creditCard.ChargeCount; }
            }

            public void Charge()
            {
                var chargeMessage = _creditCard.Charge();
                Console.WriteLine(chargeMessage);
            }
        }
    }
}
