import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-notification',
  imports: [CommonModule],
  templateUrl: './notification.html',
  styleUrl: './notification.scss'
})
export class Notification {
  @Input() isSuccess: boolean = true;
  @Input() title: string = '';
  @Input() message: string = '';
  @Input() isHtmlEnabled: boolean = false;

  constructor(public activeModal: NgbActiveModal) {}
}
