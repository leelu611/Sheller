using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sheller.Models
{
    /// <summary>
    /// A top-level interface for executables.
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// Executes the executable.
        /// </summary>
        /// <returns>A task which results in an <see cref="ICommandResult"/> (i.e., the result of the execution).</returns>
        Task<ICommandResult> ExecuteAsync();
        /// <summary>
        /// Executes the executable.
        /// </summary>
        /// <param name="resultSelector">A <see cref="Func{T}"/> which takes an <see cref="ICommandResult"/> and computes a new <typeparamref name="TResult"/> to be returned.</param>
        /// <typeparam name="TResult">The resulting type as defined by the <paramref name="resultSelector"/>.</typeparam>
        /// <returns>A task which results in a <typeparamref name="TResult"/> (i.e., the result of the execution).</returns>
        Task<TResult> ExecuteAsync<TResult>(Func<ICommandResult, TResult> resultSelector);
        /// <summary>
        /// Executes the executable.
        /// </summary>
        /// <param name="resultSelector">A <see cref="Func{T}"/> which takes an <see cref="ICommandResult"/> and computes a new <see cref="Task"/> of <typeparamref name="TResult"/> to be returned.</param>
        /// <typeparam name="TResult">The resulting type as defined by the <paramref name="resultSelector"/>.</typeparam>
        /// <returns>A task which results in a <typeparamref name="TResult"/> (i.e., the result of the execution).</returns>
        Task<TResult> ExecuteAsync<TResult>(Func<ICommandResult, Task<TResult>> resultSelector);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the same settings as the invoking instance.</returns>
        IExecutable Clone();

        /// <summary>
        /// Changes the shell of the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="shell">The new <see cref="IShell"/> to use.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the arguments passed to this call.</returns>
        IExecutable UseShell(IShell shell);

        /// <summary>
        /// Changes the executable of the execution context and returns a `new` context instance.
        /// This should be used sparingly for very specific use cases (e.g., you renamed `kubectl` to `k`, and you need to reflect that).
        /// </summary>
        /// <param name="executable">The new executable to use.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the arguments passed to this call.</returns>
        IExecutable UseExecutable(string executable);

        /// <summary>
        /// Adds an argument (which are appended space-separated to the execution command) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="args">An arbitrary list of strings to be added as parameters.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the arguments passed to this call.</returns>
        IExecutable WithArgument(params string[] args);

        /// <summary>
        /// Sets the timeout on the entire execution of this entire execution context.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the timeout set to the value passed to this call.</returns>
        IExecutable UseTimeout(TimeSpan timeout);

        /// <summary>
        /// Adds a string to the standard input stream (of which there may be many) to the executable context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardInput">A string that gets passed to the standard input stream of the executable.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the standard input passed to this call.</returns>
        IExecutable WithStandardInput(string standardInput);
        /// <summary>
        /// Adds a standard output handler (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardOutputHandler">An <see cref="Action"/> that handles a new line in the standard output of the executable.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the standard output handler passed to this call.</returns>
        IExecutable WithStandardOutputHandler(Action<string> standardOutputHandler);
        /// <summary>
        /// Adds an error output handler (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardErrorHandler">An <see cref="Action"/> that handles a new line in the standard error of the executable.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the standard error handler passed to this call.</returns>
        IExecutable WithStandardErrorHandler(Action<string> standardErrorHandler);
        /// <summary>
        /// Adds a (user) input request handler to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="inputRequestHandler">
        /// A <see cref="Func{T}"/> that handles when the shell blocks for user input during an execution.
        /// This handler should take (string StandardOutput, string StandardInput) and return a <see cref="Task{String}"/>
        /// that will be passed to the executable as StandardInput.
        /// </param>
        /// <returns>A `new` instance of <see cref="IShell"/> with the request handler passed to this call.</returns>
        IExecutable UseInputRequestHandler(Func<string, string, Task<string>> inputRequestHandler);
        /// <summary>
        /// Set the Standard Output Encoding on the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardOutputEncoding">The encoding to use for standard output.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the standard output encoding passed to this call.</returns>
        IExecutable UseStandardOutputEncoding(Encoding standardOutputEncoding);
        /// <summary>
        /// Set the Standard Error Encoding on the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardErrorEncoding">The encoding to use for standard error.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the standard error encoding passed to this call.</returns>
        IExecutable UseStandardErrorEncoding(Encoding standardErrorEncoding);

        /// <summary>
        /// Provides an <see cref="IObservable{T}"/> to which a subscription can be placed.
        /// The observable never completes, since executions can be run many times.
        /// </summary>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the subscribers attached to the observable.</returns>
        IExecutable WithSubscribe(Action<IObservable<ICommandEvent>> subscriber);

        /// <summary>
        /// Adds a <see cref="CancellationToken"/> (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the cancellation token attached.</returns>
        IExecutable WithCancellationToken(CancellationToken cancellationToken);

        /// <summary>
        /// Adds a wait <see cref="Func{T}"/> (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="waitFunc">A <see cref="Func{T}"/> which takes an <see cref="ICommandResult"/> and returns a <see cref="Task"/> which will function as wait condition upon the completion of execution.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the wait func passed to this call.</returns>
        IExecutable WithWait(Func<ICommandResult, Task> waitFunc);
        /// <summary>
        /// Sets the wait timeout on the <see cref="WithWait"/> <see cref="Func{T}"/>.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the wait timeout set to the value passed to this call.</returns>
        IExecutable UseWaitTimeout(TimeSpan timeout);

        /// <summary>
        /// Set a transform function on the <cref see="ProcessStartInfo"/> that is applied before execution, and returns a `new` context instance.
        /// </summary>
        /// <param name="startInfoTransform">The transform.</param>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> with the transform passed to this call.</returns>
        IExecutable UseStartInfoTransform(Action<ProcessStartInfo> startInfoTransform);

        /// <summary>
        /// Ensures the execution context will not throw on a non-zero exit code and returns a `new` context instance.
        /// </summary>
        /// <returns>A `new` instance of type <see cref="IExecutable"/> that will not throw on a non-zero exit code.</returns>
        IExecutable UseNoThrow();
    }

    /// <summary>
    /// A top-level interface for executables.
    /// </summary>
    /// <typeparam name="TIExecutable">The type of the executable class implementing this interface.  This allows the base class to return `new` instances for daisy chaining.</typeparam>
    public interface IExecutable<out TIExecutable> : IExecutable where TIExecutable : IExecutable
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A `new` instance of <typeparamref name="TIExecutable"/> with the same settings as the invoking instance.</returns>
        new TIExecutable Clone();

        /// <summary>
        /// Changes the shell of the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="shell">The new <see cref="IShell"/> to use.</param>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> with the arguments passed to this call.</returns>
        new TIExecutable UseShell(IShell shell);

        /// <summary>
        /// Changes the executable of the execution context and returns a `new` context instance.
        /// This should be used sparingly for very specific use cases (e.g., you renamed `kubectl` to `k`, and you need to reflect that).
        /// </summary>
        /// <param name="executable">The new executable to use.</param>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> with the arguments passed to this call.</returns>
        new TIExecutable UseExecutable(string executable);

        /// <summary>
        /// Adds an argument (which are appended space-separated to the execution command) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="args">An arbitrary list of strings to be added as parameters.</param>
        /// <returns>A `new` instance of <typeparamref name="TIExecutable"/> with the arguments passed to this call.</returns>
        new TIExecutable WithArgument(params string[] args);

        /// <summary>
        /// Sets the timeout on the entire execution of this entire execution context.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A `new` instance of <typeparamref name="TIExecutable"/> with the timeout set to the value passed to this call.</returns>
        new TIExecutable UseTimeout(TimeSpan timeout);

        /// <summary>
        /// Adds a string to the standard input stream (of which there may be many) to the executable context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardInput">A string that gets passed to the standard input stream of the executable.</param>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> with the standard input passed to this call.</returns>
        new TIExecutable WithStandardInput(string standardInput);
        /// <summary>
        /// Adds a standard output handler (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardOutputHandler">An <see cref="Action"/> that handles a new line in the standard output of the executable.</param>
        /// <returns>A `new` instance of <typeparamref name="TIExecutable"/> with the standard output handler passed to this call.</returns>
        new TIExecutable WithStandardOutputHandler(Action<string> standardOutputHandler);
        /// <summary>
        /// Adds an error output handler (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardErrorHandler">An <see cref="Action"/> that handles a new line in the standard error of the executable.</param>
        /// <returns>A `new` instance of <typeparamref name="TIExecutable"/> with the standard error handler passed to this call.</returns>
        new TIExecutable WithStandardErrorHandler(Action<string> standardErrorHandler);
        /// <summary>
        /// Adds a (user) input request handler to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="inputRequestHandler">
        /// A <see cref="Func{T}"/> that handles when the shell blocks for user input during an execution.
        /// This handler should take (string StandardOutput, string StandardInput) and return a <see cref="Task{String}"/>
        /// that will be passed to the executable as StandardInput.
        /// </param>
        /// <returns>A `new` instance of <typeparamref name="TIExecutable"/> with the request handler passed to this call.</returns>
        new TIExecutable UseInputRequestHandler(Func<string, string, Task<string>> inputRequestHandler);
        /// <summary>
        /// Set the Standard Output Encoding on the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardOutputEncoding">The encoding to use for standard output.</param>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> with the standard output encoding passed to this call.</returns>
        new TIExecutable UseStandardOutputEncoding(Encoding standardOutputEncoding);
        /// <summary>
        /// Set the Standard Error Encoding on the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="standardErrorEncoding">The encoding to use for standard error.</param>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> with the standard error encoding passed to this call.</returns>
        new TIExecutable UseStandardErrorEncoding(Encoding standardErrorEncoding);

        /// <summary>
        /// Provides an <see cref="IObservable{T}"/> to which a subscription can be placed.
        /// The observable never completes, since executions can be run many times.
        /// </summary>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> with the subscribers attached to the observable.</returns>
        new TIExecutable WithSubscribe(Action<IObservable<ICommandEvent>> subscriber);

        /// <summary>
        /// Adds a <see cref="CancellationToken"/> (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> with the cancellation token attached.</returns>
        new TIExecutable WithCancellationToken(CancellationToken cancellationToken);

        /// <summary>
        /// Adds a wait <see cref="Func{T}"/> (of which there may be many) to the execution context and returns a `new` context instance.
        /// </summary>
        /// <param name="waitFunc">A <see cref="Func{T}"/> which takes an <see cref="ICommandResult"/> and returns a <see cref="Task"/> which will function as wait condition upon the completion of execution.</param>
        /// <returns>A `new` instance of <typeparamref name="TIExecutable"/> with the wait func passed to this call.</returns>
        new TIExecutable WithWait(Func<ICommandResult, Task> waitFunc);
        /// <summary>
        /// Sets the wait timeout on the <see cref="WithWait"/> <see cref="Func{T}"/>.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A `new` instance of <typeparamref name="TIExecutable"/> with the wait timeout set to the value passed to this call.</returns>
        new TIExecutable UseWaitTimeout(TimeSpan timeout);

        /// <summary>
        /// Set a transform function on the <cref see="ProcessStartInfo"/> that is applied before execution, and returns a `new` context instance.
        /// </summary>
        /// <param name="startInfoTransform">The transform.</param>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> with the transform passed to this call.</returns>
        new TIExecutable UseStartInfoTransform(Action<ProcessStartInfo> startInfoTransform);

        /// <summary>
        /// Ensures the execution context will not throw on a non-zero exit code and returns a `new` context instance.
        /// </summary>
        /// <returns>A `new` instance of type <typeparamref name="TIExecutable"/> that will not throw on a non-zero exit code.</returns>
        new TIExecutable UseNoThrow();
    }

    /// <summary>
    /// An interface for executables that define the execute method with a special result type.
    /// </summary>
    /// <typeparam name="TIExecutable">The type of the executable class implementing this interface.  This allows the base class to return `new` instances for daisy chaining.</typeparam>
    /// <typeparam name="TResult">The result type of the executable.</typeparam>
    public interface IExecutable<out TIExecutable, TResult> : IExecutable<TIExecutable> where TIExecutable : IExecutable
    {
        /// <summary>
        /// Executes the executable.
        /// </summary>
        /// <returns>A task which results in a <typeparamref name="TResult"/> (i.e., the result of the execution).</returns>
        new Task<TResult> ExecuteAsync();
    }
}