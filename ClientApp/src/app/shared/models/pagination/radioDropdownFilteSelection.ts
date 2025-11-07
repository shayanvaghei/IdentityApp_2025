export class RadioDropdownFilteSelection {
    selectedValue: string;
    action: string;

    constructor(selectedValue: string, action: string) {
        this.selectedValue = selectedValue;
        this.action = action;
    }
}