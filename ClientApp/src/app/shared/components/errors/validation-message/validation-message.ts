import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-validation-message',
  imports: [],
  templateUrl: './validation-message.html',
  styleUrl: './validation-message.scss'
})
export class ValidationMessage {
  @Input() errorMessages: string[] | undefined;
}
