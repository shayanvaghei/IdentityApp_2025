import { CommonModule } from '@angular/common';
import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-form-input',
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './form-input.html',
  styleUrl: './form-input.scss'
})
export class FormInput implements ControlValueAccessor {
  @Input() label: string | undefined;
  @Input() type: string | undefined;
  @Input() invalidMessage: string | undefined;
  @Input() minLength: number | undefined;
  @Input() maxLength: number | undefined;
  @Input() submitted: boolean | undefined;
  displayPassword = false;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }

  writeValue(obj: any): void {
   
  }
  registerOnChange(fn: any): void {
    
  }
  registerOnTouched(fn: any): void {
    
  }
  setDisabledState?(isDisabled: boolean): void {
    
  }

  togglePasswordDisplay() {
    this.displayPassword = !this.displayPassword;
  }

}
