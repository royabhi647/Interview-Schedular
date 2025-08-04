using System.Net;
using System.Net.Mail;
using Server.Models;

namespace Server.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendInterviewNotificationAsync(Interview interview)
        {
            try
            {
                var candidateSubject = $"Interview Scheduled: {interview.JobTitle}";
                var candidateBody = GenerateInterviewEmailBody(interview, true);

                var interviewerSubject = $"Interview Scheduled with {interview.CandidateName}";
                var interviewerBody = GenerateInterviewEmailBody(interview, false);

                // Send to candidate
                await SendEmailAsync(interview.CandidateEmail, candidateSubject, candidateBody);

                // Send to interviewer
                await SendEmailAsync(interview.InterviewerEmail, interviewerSubject, interviewerBody);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendInterviewCancellationAsync(Interview interview)
        {
            try
            {
                var subject = $"Interview Cancelled: {interview.JobTitle}";
                var body = $@"
                    <h2>Interview Cancelled</h2>
                    <p>The interview for <strong>{interview.JobTitle}</strong> scheduled for {interview.StartTime:MMM dd, yyyy at HH:mm} UTC has been cancelled.</p>
                    <p>If you have any questions, please contact the interviewer directly.</p>
                ";

                await SendEmailAsync(interview.CandidateEmail, subject, body);
                await SendEmailAsync(interview.InterviewerEmail, subject, body);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
            {
                Port = int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                Credentials = new NetworkCredential(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                ),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Username"]!, "Interview Scheduler"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        private string GenerateInterviewEmailBody(Interview interview, bool isForCandidate)
        {
            var recipient = isForCandidate ? interview.CandidateName : interview.InterviewerName;
            var otherParty = isForCandidate ? "Interviewer" : "Candidate";
            var otherPartyName = isForCandidate ? interview.InterviewerName : interview.CandidateName;

            return $@"
                <h2>Interview Scheduled</h2>
                <p>Hello {recipient},</p>
                <p>Your interview has been successfully scheduled. Here are the details:</p>
                
                <div style='background-color: #f5f5f5; padding: 20px; margin: 20px 0; border-radius: 5px;'>
                    <h3>Interview Details</h3>
                    <p><strong>Position:</strong> {interview.JobTitle}</p>
                    <p><strong>Date & Time:</strong> {interview.StartTime:MMM dd, yyyy at HH:mm} UTC</p>
                    <p><strong>Duration:</strong> {(interview.EndTime - interview.StartTime).TotalMinutes} minutes</p>
                    <p><strong>{otherParty}:</strong> {otherPartyName}</p>
                    {(string.IsNullOrEmpty(interview.GoogleMeetLink) ? "" : $"<p><strong>Meeting Link:</strong> <a href='{interview.GoogleMeetLink}'>Join Google Meet</a></p>")}
                </div>

                <p>Please make sure to:</p>
                <ul>
                    <li>Join the meeting 5 minutes early</li>
                    <li>Test your camera and microphone beforehand</li>
                    <li>Have a stable internet connection</li>
                    <li>Prepare any questions you may have</li>
                </ul>

                <p>If you need to reschedule or have any questions, please contact us immediately.</p>
                
                <p>Good luck!</p>
                <p>Interview Scheduler Team</p>
            ";
        }
    }
}