namespace ViewModelToolkitSample.Services;

public static class AlertService
{
    public static async Task AlertInvalidLoyaltyPointsInputAsync(string response) {
        await App.Current.MainPage.DisplayAlert(
            "Edit Loyalty Points",
            $"\"{response.Trim()}\" is not a valid entry. Please enter an integer value.",
            "OK");
    }

    public static async Task<string> PromptForPointsAsync(int loyaltyPoints) {
        return await App.Current.MainPage.DisplayPromptAsync(
            "Edit Loyalty Points",
            "Enter the new Loyalty Points total:",
            keyboard: Keyboard.Numeric,
            initialValue: loyaltyPoints.ToString());
    }
}

