namespace MADOC.Application.Printing;

public interface IPrintTemplateProvider
{
    string GetTemplate(string templateFileName);
}
