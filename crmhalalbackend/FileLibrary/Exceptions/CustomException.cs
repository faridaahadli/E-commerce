using System;

namespace FileLibrary.Exceptions
{
  public class CustomException : Exception
  {
    public CustomException()
    {
    }

    public CustomException(string message)
      : base(message)
    {
    }
  }
}
