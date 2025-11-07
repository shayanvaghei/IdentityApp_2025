import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { AdminService } from '../../../../admin/admin.service';
import { RadioDropdownFilteSelection } from '../../../models/pagination/radioDropdownFilteSelection';

@Component({
  selector: 'app-ia-pagination-dropdown-radio-filter',
  imports: [ReactiveFormsModule],
  templateUrl: './ia-pagination-dropdown-radio-filter.html',
  styleUrl: './ia-pagination-dropdown-radio-filter.scss'
})
export class IaPaginationDropdownRadioFilter implements OnInit {
  @Input() type: string | undefined;
  @Input() entries: string[] | undefined;
  @Input() label: string | undefined;
  @Input() defaultValue: string | undefined;
  @Input() action: string | undefined;
  @Output() selectedValue = new EventEmitter<{}>();
  form: FormGroup = new FormGroup({});

  constructor(private adminService: AdminService,
    private formBuilder: FormBuilder) {
  }

  ngOnInit(): void {
    this.initializeForm();

    this.adminService.resetFilter$.subscribe(_ => {
      this.form.controls['myfilter'].setValue(this.defaultValue);
    });
  }

  initializeForm() {
    this.form = this.formBuilder.group({
      myfilter: [this.defaultValue]
    });
  }

  onEntriesSelection(event: any) {
    let selectedValue;

    if (this.type == 'dropdown') {
      selectedValue = event.target.value;
    } else {
      selectedValue = event;
    }

    if (this.action) {
      this.selectedValue.emit(new RadioDropdownFilteSelection(selectedValue, this.action));
    }
  }
}
